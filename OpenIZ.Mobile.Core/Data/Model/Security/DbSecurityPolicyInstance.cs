/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */
using System;
using SQLite.Net;
using SQLite.Net.Attributes;
using OpenIZ.Core.Data.QueryBuilder.Attributes;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Acts;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Represents a security policy instance which includes a link to a policy and
	/// to a decision
	/// </summary>
	public abstract class DbSecurityPolicyInstance : DbIdentified
	{

		/// <summary>
		/// Gets or sets the type of the grant.
		/// </summary>
		/// <value>The type of the grant.</value>
		[Column("grant_type"), NotNull]
		public int GrantType {
			get;
			set;
		}
	}



    /// <summary>
    /// Represents a security policy applied to an act
    /// </summary>
    [Table("act_security_policy")]
	public class DbActSecurityPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("act_id"), Indexed(Name = "act_security_policy_source_policy", Unique = true), NotNull, ForeignKey(typeof(DbAct), nameof(DbAct.Uuid))]
        public byte[] ActId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "act_security_policy_source_policy", Unique = true), NotNull, ForeignKey(typeof(DbSecurityPolicy), nameof(DbSecurityPolicy.Uuid))]
        public byte[] PolicyId
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Represents a security policy applied to a role
    /// </summary>
    [Table("security_role_policy")]
	public class DbSecurityRolePolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("role_id"), Indexed(Name = "security_role_policy_source_policy", Unique = true), NotNull, MaxLength(16), ForeignKey(typeof(DbSecurityRole), nameof(DbSecurityRole.Uuid))]
        public byte[] RoleId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "security_role_policy_source_policy", Unique = true), NotNull, MaxLength(16), ForeignKey(typeof(DbSecurityPolicy), nameof(DbSecurityPolicy.Uuid))]
        public byte[] PolicyId
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Represents a security policy applied to an application (this is "my" data)
	/// </summary>
	[Table("security_application_policy")]
	public class DbSecurityApplicationPolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("application_id"), Indexed(Name = "security_application_policy_source_policy", Unique = true), NotNull, MaxLength(16), ForeignKey(typeof(DbSecurityApplication), nameof(DbSecurityApplication.Uuid))]
        public byte[] ApplicationId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "security_application_policy_source_policy", Unique = true), NotNull, MaxLength(16), ForeignKey(typeof(DbSecurityPolicy), nameof(DbSecurityPolicy.Uuid))]
        public byte[] PolicyId
        {
            get;
            set;
        }
    }

	/// <summary>
	/// Represents a security policy applied to a device
	/// </summary>
	[Table("security_device_policy")]
	public class DbSecurityDevicePolicy : DbSecurityPolicyInstance
	{
        /// <summary>
        /// Gets or sets the source
        /// </summary>
        /// <value>The source identifier.</value>
        [Column("device_id"), Indexed(Name = "security_device_policy_source_policy", Unique = true), NotNull, MaxLength(16), ForeignKey(typeof(DbSecurityDevice), nameof(DbSecurityDevice.Uuid))]
        public byte[] DeviceId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the policy identifier.
        /// </summary>
        /// <value>The policy identifier.</value>
        [Column("policy_id"), Indexed(Name = "security_device_policy_source_policy", Unique = true), NotNull, MaxLength(16), ForeignKey(typeof(DbSecurityPolicy), nameof(DbSecurityPolicy.Uuid))]
        public byte[] PolicyId
        {
            get;
            set;
        }
    }
}

