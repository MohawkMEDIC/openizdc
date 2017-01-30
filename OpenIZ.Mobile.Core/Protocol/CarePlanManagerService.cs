/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-10-11
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
using System.Linq.Expressions;
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

        // Represents a promise to perform a care plan
        private HashSet<Guid> m_actCarePlanPromise = new HashSet<Guid>();

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
                try
                {
                    ApplicationContext.Current.SetProgress(Strings.locale_start_careplan, 0);

                    // Warehouse service
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
                            foreach (var i in e.Data.Item.OfType<Act>())
                                this.UpdateCarePlan(i);
                        };
                        bundlePersistence.Updated += (o, e) =>
                        {
                            foreach (var i in e.Data.Item.OfType<Patient>())
                                this.UpdateCarePlan(i);
                            foreach (var i in e.Data.Item.OfType<Act>())
                                this.UpdateCarePlan(i);
                        };
                        bundlePersistence.Obsoleted += (o, e) =>
                        {
                            var warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
                            var dataMart = warehouseService.GetDatamart("oizcp");
                            foreach (var i in e.Data.Item.OfType<Patient>())
                                warehouseService.Delete(dataMart.Id, new { patient_id = i.Key.Value });
                            foreach (var i in e.Data.Item.OfType<Act>())
                                this.UpdateCarePlan(i);
                        };

                    }

                    // Act persistence services
                    foreach (var t in typeof(Act).GetTypeInfo().Assembly.ExportedTypes.Where(o => o == typeof(Act) || typeof(Act).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && !o.GetTypeInfo().IsAbstract))
                    {
                        var pType = typeof(IDataPersistenceService<>).MakeGenericType(t);
                        var pInstance = ApplicationContext.Current.GetService(pType) as IDataPersistenceService;

                        // Create a delegate which calls UpdateCarePlan
                        // Construct the delegate
                        var ppeArgType = typeof(DataPersistenceEventArgs<>).MakeGenericType(t);
                        var evtHdlrType = typeof(EventHandler<>).MakeGenericType(ppeArgType);
                        var senderParm = Expression.Parameter(typeof(Object), "o");
                        var eventParm = Expression.Parameter(ppeArgType, "e");
                        var eventData = Expression.Convert(Expression.MakeMemberAccess(eventParm, ppeArgType.GetRuntimeProperty("Data")), typeof(Act));
                        var insertInstanceDelegate = Expression.Lambda(evtHdlrType, Expression.Call(Expression.Constant(this), typeof(CarePlanManagerService).GetRuntimeMethod(nameof(UpdateCarePlan), new Type[] { typeof(Act) }), eventData), senderParm, eventParm).Compile();
                        var updateInstanceDelegate = Expression.Lambda(evtHdlrType, Expression.Call(Expression.Constant(this), typeof(CarePlanManagerService).GetRuntimeMethod(nameof(UpdateCarePlan), new Type[] { typeof(Act) }), eventData), senderParm, eventParm).Compile();
                        var obsoleteInstanceDelegate = Expression.Lambda(evtHdlrType, Expression.Call(Expression.Constant(this), typeof(CarePlanManagerService).GetRuntimeMethod(nameof(UpdateCarePlan), new Type[] { typeof(Act) }), eventData), senderParm, eventParm).Compile();

                        // Bind to events
                        pType.GetRuntimeEvent("Inserted").AddEventHandler(pInstance, insertInstanceDelegate);
                        pType.GetRuntimeEvent("Updated").AddEventHandler(pInstance, updateInstanceDelegate);
                        pType.GetRuntimeEvent("Obsoleted").AddEventHandler(pInstance, obsoleteInstanceDelegate);

                    }

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Could not bind clinical protocols: {0}", e);
                }
            };
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Update the care plan given that a new act exists
        /// </summary>
        public void UpdateCarePlan(Act a)
        {
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem((d) =>
            {
                var act = d as Act;

                // First step, we delete all acts in the warehouse for the specified patient in the protocol
                var patientId = act.Participations.FirstOrDefault(o => o.ParticipationRoleKey == ActParticipationKey.RecordTarget)?.PlayerEntityKey;
                if (patientId == null)
                {
                    this.m_tracer.TraceWarning("Cannot update care plan for act as it seems to have no RecordTarget");
                    return;
                }

                try
                {

                    IEnumerable<Act> carePlan = null;
                    var careplanService = ApplicationContext.Current.GetService<ICarePlanService>();
                    var warehouseService = ApplicationContext.Current.GetService<IAdHocDatawarehouseService>();
                    var dataMart = warehouseService.GetDatamart("oizcp");
                    var patient = this.EnsureParticipations(ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>().Get(patientId.Value).Clone() as Patient);
                     

                    // Is there a protocol for this act?
                    if (act.Protocols.Count() == 0)
                    {
                        if (this.m_actCarePlanPromise.Contains(patient.Key.Value)) return;
                        else lock (this.m_actCarePlanPromise)
                            {
                                if (this.m_actCarePlanPromise.Contains(patient.Key.Value)) return;
                                else this.m_actCarePlanPromise.Add(patient.Key.Value);
                            }

                        // Need to re-calculate the entire care plan
                        this.m_tracer.TraceWarning("Will need to calculate the entire care plan for patient {0}", patientId);

                        this.m_tracer.TraceVerbose("Calculating care plan for {0}", patient.Key);

                        // First, we clear the warehouse
                        warehouseService.Delete(dataMart.Id, new { patient_id = patient.Key.Value });

                        // Now calculate
                        carePlan = careplanService.CreateCarePlan(patient, false);
                    }
                    else
                    {
                        warehouseService.Delete(dataMart.Id, new
                        {
                            patient_id = patientId.Value,
                            protocol_id = act.Protocols.FirstOrDefault()?.ProtocolKey
                        });

                        carePlan = careplanService.CreateCarePlan(patient, false, act.Protocols.FirstOrDefault().ProtocolKey);
                    }


                    /// Create a plan for the warehouse
                    var warehousePlan = carePlan.Select(o => new
                    {
                        creation_date = DateTime.Now,
                        patient_id = patient.Key.Value,
                        location_id = patient.Relationships.FirstOrDefault(r => r.RelationshipTypeKey == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation || r.RelationshipType?.Mnemonic == "DedicatedServiceDeliveryLocation")?.TargetEntityKey.Value,
                        act_id = o.Key.Value,
                        protocol_id = o.Protocols.FirstOrDefault().ProtocolKey,
                        class_id = o.ClassConceptKey.Value,
                        type_id = o.TypeConceptKey.Value,
                        min_date = o.StartTime?.DateTime,
                        max_date = o.StopTime?.DateTime,
                        act_date = o.ActTime.DateTime,
                        product_id = o.Participations?.FirstOrDefault(r => r.ParticipationRoleKey == ActParticipationKey.Product || r.ParticipationRole?.Mnemonic == "Product")?.PlayerEntityKey.Value
                    });

                    // Insert plans
                    warehouseService.Add(dataMart.Id, warehousePlan);
                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Could not update care plan based on Act {0}: {1}", act, ex);
                }
                finally
                {
                    lock (this.m_actCarePlanPromise)
                        this.m_actCarePlanPromise.Remove(patientId.Value);
                }
            }, a.Clone());

        }

        /// <summary>
        /// Update the care plan for the specified patient
        /// </summary>
        private void UpdateCarePlan(Patient p)
        {
            ApplicationContext.Current.GetService<IThreadPoolService>().QueueUserWorkItem((d) =>
            {
                var data = d as Patient;

                try
                {
                    if (this.m_actCarePlanPromise.Contains(data.Key.Value)) return;
                    else lock (this.m_actCarePlanPromise)
                        {
                            if (this.m_actCarePlanPromise.Contains(data.Key.Value)) return;
                            else this.m_actCarePlanPromise.Add(data.Key.Value);
                        }


                    this.m_tracer.TraceVerbose("Calculating care plan for {0}", data.Key);
                    data = this.EnsureParticipations(data);

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

                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Could not update care plan for Patient {0}: {1}", p, ex);
                }
                finally
                {
                    lock (this.m_actCarePlanPromise)
                        this.m_actCarePlanPromise.Remove(data.Key.Value);
                }

            }, p.Clone());
        }

        /// <summary>
        /// Ensure participations are loaded
        /// </summary>
        private Patient EnsureParticipations(Patient patient)
        {
            patient = patient.Clone() as Patient;
            var actService = ApplicationContext.Current.GetService<IActRepositoryService>();
            int tr = 0;
            IEnumerable<Act> acts = null;
            Guid searchState = Guid.Empty;

            acts = actService.Find<Act>(o => o.Participations.Any(guard => guard.ParticipationRole.Mnemonic == "RecordTarget" && guard.PlayerEntityKey == patient.Key), 0, 200, out tr);

            patient.Participations = acts.Select(a => new ActParticipation(ActParticipationKey.RecordTarget, patient) { Act = a, ParticipationRole = new OpenIZ.Core.Model.DataTypes.Concept() { Mnemonic = "RecordTarget" } }).ToList();
            return patient;
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
