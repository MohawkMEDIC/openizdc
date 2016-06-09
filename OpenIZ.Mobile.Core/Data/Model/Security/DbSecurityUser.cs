﻿using System;
using SQLite;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Mobile.Core.Data.Model.Security
{
	/// <summary>
	/// Represents a user for the purpose of authentication
	/// </summary>
	[Table("security_user")]
	public class DbSecurityUser : DbBaseData
	{

		/// <summary>
		/// Gets or sets the email.
		/// </summary>
		/// <value>The email.</value>
		[Column("email")]
		public String Email {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the invalid login attempts.
		/// </summary>
		/// <value>The invalid login attempts.</value>
		[Column("invalid_logins")]
		public int InvalidLoginAttempts {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="OpenIZ.Mobile.Core.Data.Model.Security.DbSecurityUser"/> lockout enabled.
		/// </summary>
		/// <value><c>true</c> if lockout enabled; otherwise, <c>false</c>.</value>
		[Column("locked")]
		public DateTime? Lockout {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the password hash.
		/// </summary>
		/// <value>The password hash.</value>
		[Column("password"), NotNull, Indexed]
		public String PasswordHash {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the security stamp.
		/// </summary>
		/// <value>The security stamp.</value>
		[Column("security_stamp")]
		public String SecurityHash {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is two factor enabled.
		/// </summary>
		/// <value><c>true</c> if this instance is two factor enabled; otherwise, <c>false</c>.</value>
		[Column("tfa_enabled")]
		public bool TwoFactorEnabled {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the name of the user.
		/// </summary>
		/// <value>The name of the user.</value>
		[Column("username"), Unique, NotNull]
		public String UserName {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the last login.
		/// </summary>
		/// <value>The last login.</value>
		[Column("last_login")]
		public DateTime? LastLoginTime {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the phone number.
		/// </summary>
		/// <value>The phone number.</value>
		[Column("phone_number")]
		public String PhoneNumber {
			get;
			set;
		}

	}

	/// <summary>
	/// Associative entity between security user and role
	/// </summary>
	[Table("security_user_role")]
	public class DbSecurityUserRole : DbIdentified
	{
		/// <summary>
		/// Gets or sets the role identifier
		/// </summary>
		/// <value>The role identifier.</value>
		[Column("role_id"), Indexed(Name = "security_user_role_user_role", Unique = true), MaxLength(16)]
		public byte[] RoleUuid {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the user identifier
		/// </summary>
		/// <value>The user identifier.</value>
		[Column("user_id"), Indexed(Name = "security_user_role_user_role", Unique = true), MaxLength(16)]
		public byte[] UserUuid {
			get;
			set;
		}


	}
}
