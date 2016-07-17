using System;
using System.Security.Principal;

namespace OpenIZ.Mobile.Core.Services
{
	/// <summary>
	/// Represents a service which is capableof retrieving roles
	/// </summary>
	public interface IRoleProviderService
	{

		/// <summary>
		/// Find all users in a role
		/// </summary>
		string[] FindUsersInRole(string role);

		/// <summary>
		/// Get all roles
		/// </summary>
		/// <returns></returns>
		string[] GetAllRoles();

		/// <summary>
		/// Get all rolesfor user
		/// </summary>
		/// <returns></returns>
		string[] GetAllRoles(String userName);

		/// <summary>
		/// Determine if the user is in the specified role
		/// </summary>
		bool IsUserInRole(IPrincipal principal, string roleName);

		/// <summary>
		/// Determine if user is in role
		/// </summary>
		bool IsUserInRole(string userName, string roleName);

        /// <summary>
        /// Adds the specified users to the specified roles
        /// </summary>
        void AddUsersToRoles(string[] userNames, string[] roleNames);

	}
}

