/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-1-16
 */
using OpenIZ.Core.Data.Warehouse;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Resources;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Protocol
{
    /// <summary>
    /// The protocol watch service is used to watch patient regsitrations and ensure the clinical
    /// protocol is complete
    /// </summary>
    public class CarePlanManagerService : IDaemonService
    {

        // Warehouse service
        private IAdHocDatawarehouseService m_warehouseService;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(CarePlanManagerService));

        /// <summary>
        /// True when running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Fired when the watch is starting
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the watch is Starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the watch has stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Fired when the watch is stopping
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Start the daemon service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            this.m_tracer.TraceInfo("Starting care plan manager / warehousing service...");

            // Application context has started
            ApplicationContext.Current.Started += (ao, ae) =>
            {
                ApplicationContext.Current.SetProgress(Strings.locale_start_careplan, 0);

                // Warehouse service
                try
                {
                    this.m_warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
                    foreach (var cp in ApplicationContext.Current.GetService<ICarePlanService>().Protocols)
                        this.m_tracer.TraceInfo("Loaded {0}...", cp.Name);
                    // Deploy schema?
                    if (this.m_warehouseService.GetDatamart("oizcp") == null)
                    {
                        this.m_tracer.TraceInfo("Datamart for care plan service doesn't exist, will have to create it...");
                        var dm = this.m_warehouseService.CreateDatamart("oizcp", DatamartSchema.Load(typeof(CarePlanManagerService).GetTypeInfo().Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Protocol.CarePlanWarehouseSchema.xml")));
                        this.m_tracer.TraceVerbose("Datamart {0} created", dm.Id);
                    }

                    // Subscribe to persistence
                    var patientPersistence = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
                    if (patientPersistence != null)
                    {
                        patientPersistence.Inserted += (o, e) => this.UpdateCarePlan(e.Data);
                        patientPersistence.Updated += (o, e) => this.UpdateCarePlan(e.Data);
                        patientPersistence.Obsoleted += (o, e) =>
                        {
                            var warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
                            var dataMart = warehouseService.GetDatamart("oizcp");
                            warehouseService.Delete(dataMart.Id, new { patient_id = e.Data.Key.Value });
                        };

                    }

                    // Subscribe to acts
                    var bundlePersistence = ApplicationContext.Current.GetService<IDataPersistenceService<Bundle>>();
                    if (bundlePersistence != null)
                    {
                        bundlePersistence.Inserted += (o, e) =>
                        {
                            foreach (var i in e.Data.Item.OfType<Patient>())
                                this.UpdateCarePlan(i);
                        };
                        bundlePersistence.Updated += (o, e) =>
                        {
                            foreach (var i in e.Data.Item.OfType<Patient>())
                                this.UpdateCarePlan(i);
                        };
                        bundlePersistence.Obsoleted += (o, e) =>
                        {
                            var warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
                            var dataMart = warehouseService.GetDatamart("oizcp");
                            foreach (var i in e.Data.Item.OfType<Patient>())
                                warehouseService.Delete(dataMart.Id, new { patient_id = i.Key.Value });
                        };

                    }
                }
                catch(Exception e)
                {
                    this.m_tracer.TraceError("Error binding to care plan service: {0}", e);
                }

            };
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Update the care plan for the specified patient
        /// </summary>
        private void UpdateCarePlan(Patient p)
        {
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem((d) =>
            {
                var data = d as Patient;

                data.Participations = new List<ActParticipation>(data.Participations);
                this.m_tracer.TraceVerbose("Calculating care plan for {0}", data.Key);

                // First, we clear the warehouse
                var warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
                var dataMart = warehouseService.GetDatamart("oizcp");
                warehouseService.Delete(dataMart.Id, new { patient_id = data.Key.Value });
                var careplanService = ApplicationContext.Current.GetService<ICarePlanService>();

                // Now calculate
                var carePlan = careplanService.CreateCarePlan(data, false);
                var warehousePlan = carePlan.Select(o => new
                {
                    creation_date = DateTime.Now,
                    patient_id = data.Key.Value,
                    location_id = data.Relationships.FirstOrDefault(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation || r.RelationshipType?.Mnemonic == "DedicatedServiceDeliveryLocation")?.TargetEntityKey.Value,
                    act_id = o.Key.Value,
                    class_id = o.ClassConceptKey.Value,
                    type_id = o.TypeConceptKey.Value,
                    protocol_id = o.Protocols.FirstOrDefault()?.ProtocolKey,
                    min_date = o.StartTime?.DateTime,
                    max_date = o.StopTime?.DateTime,
                    act_date = o.ActTime.DateTime,
                    product_id = o.Participations?.FirstOrDefault(r => r.ParticipationRoleKey == ActParticipationKey.Product || r.ParticipationRole?.Mnemonic == "Product")?.PlayerEntityKey.Value
                });

                // Insert plans
                warehouseService.Add(dataMart.Id, warehousePlan);
            }, p.Clone());
        }

        /// <summary>
        /// Stops the daemon service
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
