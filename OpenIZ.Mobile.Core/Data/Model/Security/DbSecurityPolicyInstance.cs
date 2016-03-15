using System;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Security
{
	/// <summary>
	/// Represents a security policy instance which includes a link to a policy and
	/// to a decision
	/// </summary>
	public abstract class DbSecurityPolicyInstance : DbIdentified
	{

		/// <summary>
		/// Gets or sets the source
		/// </summary>
		/// <value>The source identifier.</value>
		[Column("source_id"), Indexed, NotNull]
		public int SourceId {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the policy identifier.
		/// </summary>
		/// <value>The policy identifier.</value>
		[Column("policy_id"), NotNull]
		public int PolicyId {
			get;
			set;
		}

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
	/// Represents a relationship between an entity and security policy
	/// </summary>
	[Table("entity_security_policy")]
	public class DbEntitySecurityPolicy : DbSecurityPolicyInstance
	{
	}

	/// <summary>
	/// Represents a security policy applied to an act
	/// </summary>
	[Table("act_security_policy")]
	public class DbActSecurityPolicy : DbSecurityPolicyInstance
	{
	}

	/// <summary>
	/// Represents a security policy applied to a role
	/// </summary>
	[Table("security_role_policy")]
	public class DbSecurityRolePolicy : DbSecurityPolicyInstance
	{
	}

	/// <summary>
	/// Represents a security policy applied to an application (this is "my" data)
	/// </summary>
	[Table("security_application_policy")]
	public class DbSecurityApplicationPolicy : DbSecurityPolicyInstance
	{
	}

	/// <summary>
	/// Represents a security policy applied to a device
	/// </summary>
	[Table("security_device_policy")]
	public class DbSecurityDevicePolicy : DbSecurityPolicyInstance
	{
	}
}

