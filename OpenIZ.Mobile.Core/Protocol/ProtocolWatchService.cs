﻿using OpenIZ.Core.Model;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Protocol
{
    /// <summary>
    /// The protocol watch service is used to watch patient regsitrations and ensure the clinical
    /// protocol is complete
    /// </summary>
    public class ProtocolWatchService : IDaemonService
    {
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

            // Application context has started
            ApplicationContext.Current.Started += (ao, ae) => {
                var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();
                if(persistence != null)
                    persistence.Inserted += (o, e) => {

                        // We want to make sure the care plan contains everything in the protocols as possible
                        var cpService = ApplicationContext.Current.GetService<ICarePlanService>();
                        var acts = cpService.CreateCarePlan(e.Data);

                        // There were some acts proposed
                        if (acts.Count() > 0)
                        {
                            var actService = ApplicationContext.Current.GetService<IBatchRepositoryService>();
                            Bundle batch = new Bundle()
                            {
                                Item = acts.OfType<IdentifiedData>().ToList()
                            };
                            actService.Insert(batch);
                        }
                    };
            };
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
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