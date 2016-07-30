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
 * Date: 2016-7-23
 */
using System;
using System.Linq;
using OpenIZ.Mobile.Core.Configuration;
using System.Collections.Generic;
using OpenIZ.Core.Model.Security;
using System.Security.Principal;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Services;
using SQLite.Net;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Data.Model.Security;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Data.Connection;

namespace OpenIZ.Mobile.Core.Security
{
	/// <summary>
	/// Represents a PIP which uses the local data store
	/// </summary>
	public class LocalPolicyInformationService : IPolicyInformationService
	{
        // Configuration
        private DataConfigurationSection m_configuration = ApplicationContext.Current.Configuration.GetSection<DataConfigurationSection>();

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LocalPolicyInformationService));

        /// <summary>
        /// Creates a connection to the local database
        /// </summary>
        /// <returns>The connection.</returns>
        private SQLiteConnectionWithLock CreateConnection()
        {
            return SQLiteConnectionManager.Current.GetConnection(ApplicationContext.Current.Configuration.GetConnectionString(this.m_configuration.MainDataSourceConnectionStringName).Value);
        }
        
        /// <summary>
        /// Get active policies for the specified securable type
        /// </summary>
        public IEnumerable<IPolicyInstance> GetActivePolicies(object securable)
		{
			// Security device
			if (securable is SecurityDevice)
				throw new NotImplementedException ();
			else if (securable is SecurityRole)
				throw new NotImplementedException ();
			else if (securable is SecurityApplication)
				throw new NotImplementedException ();
			else if (securable is IPrincipal || securable is IIdentity) {
				var identity = (securable as IPrincipal)?.Identity ?? securable as IIdentity;

                // Is the identity a claims identity? If yes, we just use the claims made in the policy
                if(identity is ClaimsIdentity && (identity as ClaimsIdentity).Claim.Any(o=>o.Type == ClaimTypes.OpenIzGrantedPolicyClaim))
                    return (identity as ClaimsIdentity).Claim.Where(o => o.Type == ClaimTypes.OpenIzGrantedPolicyClaim).Select(
                        o=> new GenericPolicyInstance(new GenericPolicy(o.Value, "ClaimPolicy", false), PolicyGrantType.Grant)
                        );

                var conn = this.CreateConnection();
                using (conn.Lock())
                {
                    var policyRaw = conn.Query<DbSecurityPolicy>("SELECT security_policy.* FROM security_user_role INNER JOIN security_role_policy ON (security_role_policy.role_id = security_user_role.role_id) INNER JOIN security_policy ON (security_policy.uuid = security_role_policy.policy_id) INNER JOIN security_user ON (security_user_role.user_id = security_user.uuid) WHERE security_user.username = ? and grant_type = 2",
                        identity.Name).ToList();
                    return policyRaw.Select(o => new GenericPolicyInstance(new GenericPolicy(o.Oid, o.Name, o.CanOverride), PolicyGrantType.Grant));

                }
            } else if (securable is Act) {
				var pAct = securable as Act;
				throw new NotImplementedException ();

			} else if (securable is Entity) {
				throw new NotImplementedException ();
			} else
				return new List<IPolicyInstance> ();
		}

		/// <summary>
		/// Get all policies on the system
		/// </summary>
		public IEnumerable<IPolicy> GetPolicies()
		{
			return null;
		}

		/// <summary>
		/// Get a specific policy
		/// </summary>
		public IPolicy GetPolicy(string policyOid)
		{
            var conn = this.CreateConnection();
                using(conn.Lock())
            {
                var dbp = conn.Table<DbSecurityPolicy>().Where(o => o.Oid == policyOid).FirstOrDefault();
                if (dbp == null) return null;
                else return new GenericPolicy(dbp.Oid, dbp.Name, dbp.CanOverride);
            }
		}

        /// <summary>
        /// Create the policy locally
        /// </summary>
        public void CreatePolicy(IPolicy policy, IPrincipal principal)
        {
            // Demand local admin
            var pdp = ApplicationContext.Current.GetService<IPolicyDecisionService>();
            if (pdp.GetPolicyOutcome(principal ?? ApplicationContext.Current.Principal, PolicyIdentifiers.AccessClientAdministrativeFunction) != PolicyGrantType.Grant)
                throw new PolicyViolationException(PolicyIdentifiers.AccessClientAdministrativeFunction, PolicyGrantType.Deny);

            var conn = this.CreateConnection();
            using (conn.Lock())
            {
                try
                {
                    var polId = conn.Table<DbSecurityPolicy>().Where(o => o.Oid == policy.Oid).FirstOrDefault();
                    if (polId == null)
                    {
                        polId = new DbSecurityPolicy()
                        {
                            CanOverride = policy.CanOverride,
                            Name = policy.Name,
                            Oid = policy.Oid,
                            Key = Guid.NewGuid()
                        };
                        conn.Insert(polId);
                    }

                }
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Could create policy {0}", e);
                }
            }
        }
    }
}

