using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Core.Model;
using OpenIZ.Mobile.Core.Android.Security;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;

namespace OpenIZ.Mobile.Core.Android.Subscribers
{
    /// <summary>
    /// Subscriber which performs policy enforcement
    /// </summary>
    public class PolicyEnforcementSubscriber : IDaemonService
    {
        // Running flag
        private bool m_isRunning = false;

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(PolicyEnforcementSubscriber));
        /// <summary>
        /// Returns true when the daemon is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_isRunning;
            }
        }

        /// <summary>
        /// Fired when the subscriber has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the subscriber is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the subscriber is stopped
        /// </summary>
        public event EventHandler Stopped;
        /// <summary>
        /// Fired when the subscriber is stopping
        /// </summary>
        public event EventHandler Stopping;

        /// <summary>
        /// Starts the subscriber
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            try
            {
                // Attach the subscriptions here
                foreach (var t in typeof(Entity).Assembly.GetTypes().Where(t => typeof(Entity).IsAssignableFrom(t) || typeof(Act).IsAssignableFrom(t) || typeof(Concept).IsAssignableFrom(t)))
                {
                    var idpType = typeof(IDataPersistenceService<>).MakeGenericType(new Type[] { t });
                    var idpInstance = ApplicationContext.Current.GetService(idpType);
                    var mi = typeof(PolicyEnforcementSubscriber).GetMethod(nameof(BindClinicalEnforcement)).MakeGenericMethod(new Type[] { t });
                    mi.Invoke(this, new object[] { idpInstance });
                }

                this.Started?.Invoke(this, EventArgs.Empty);
                this.m_isRunning = true;
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error starting policy enformcent point: {0}", e);
            }
            return this.m_isRunning;
        }

        /// <summary>
        /// Bind the enforcement point
        /// </summary>
        protected void BindClinicalEnforcement<TData>(IDataPersistenceService<TData> persister) where TData : IdentifiedData
        {

            // Demand query
            persister.Querying += (o, e) =>
            {
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PolicyIdentifiers.QueryClinicalData).Demand();
            };

            // Demand insert
            persister.Inserting += (o, e) =>
            {
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PolicyIdentifiers.WriteClinicalData).Demand();
            };

            // Demand update
            persister.Updating += (o, e) =>
            {
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PolicyIdentifiers.WriteClinicalData).Demand();
            };

            // Obsoletion permission demand
            persister.Obsoleting += (o, e) =>
            {
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PolicyIdentifiers.DeleteClinicalData).Demand();
            };

            // Queried data filter
            persister.Queried += (o, e) =>
            {
                new PolicyPermission(System.Security.Permissions.PermissionState.Unrestricted, PolicyIdentifiers.ReadClinicalData).Demand();
                DataQueryResultEventArgs<TData> dqre = e as DataQueryResultEventArgs<TData>;
                // Filter dataset
                if(dqre != null)
                    dqre.Results = dqre.Results.Where(i => ApplicationContext.Current.PolicyDecisionService.GetPolicyDecision(ApplicationContext.Current.Principal, i) == OpenIZ.Core.Model.Security.PolicyGrantType.Grant);
            };
        }

        /// <summary>
        /// Stop the execution of the enforcement daemon
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.m_isRunning = false;
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}