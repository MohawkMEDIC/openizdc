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
 * User: khannan
 * Date: 2017-1-20
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.AMI.Alerting;
using OpenIZ.Core.Model.AMI.Applet;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model.AMI.DataTypes;
using OpenIZ.Core.Model.AMI.Diagnostics;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Security;

namespace OpenIZ.Mobile.Core.Services
{
	/// <summary>
	/// Represents an administrative integration interface.
	/// </summary>
	public interface IAdministrationService
	{
		/// <summary>
		/// Accepts a certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be accepted.</param>
		/// <returns>Returns the acceptance result.</returns>
		SubmissionResult AcceptCsr(string id);

		/// <summary>
		/// Changes the password of a user.
		/// </summary>
		/// <param name="id">The id of the user whose password is to be changed.</param>
		/// <param name="password">The new password of the user.</param>
		/// <returns>Returns the updated user.</returns>
		[Obsolete("Use UpdateUser instead as this RPC style call may be removed in the future")]
		SecurityUser ChangePassword(String id, string password);

		/// <summary>
		/// Creates an alert.
		/// </summary>
		/// <param name="alertMessageInfo">The alert message to be created.</param>
		/// <returns>Returns the created alert.</returns>
		AlertMessageInfo CreateAlert(AlertMessageInfo alertMessageInfo);

		/// <summary>
		/// Creates an applet.
		/// </summary>
		/// <param name="appletManifestInfo">The applet manifest info to be created.</param>
		/// <returns>Returns the created applet manifest info.</returns>
		AppletManifestInfo CreateApplet(AppletManifestInfo appletManifestInfo);

		/// <summary>
		/// Creates a security application.
		/// </summary>
		/// <param name="applicationInfo">The security application to be created.</param>
		/// <returns>Returns the created security application.</returns>
		SecurityApplicationInfo CreateApplication(SecurityApplicationInfo applicationInfo);

		/// <summary>
		/// Creates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityInfo">The assigning authority to be created.</param>
		/// <returns>Returns the created assigning authority.</returns>
		AssigningAuthorityInfo CreateAssigningAuthority(AssigningAuthorityInfo assigningAuthorityInfo);

		/// <summary>
		/// Creates a device in the IMS.
		/// </summary>
		/// <param name="deviceInfo">The device to be created.</param>
		/// <returns>Returns the newly created device.</returns>
		SecurityDeviceInfo CreateDevice(SecurityDeviceInfo deviceInfo);

		/// <summary>
		/// Creates a diagnostic report.
		/// </summary>
		/// <param name="report">The diagnostic report to be created.</param>
		/// <returns>Returns the created diagnostic report.</returns>
		DiagnosticReport CreateDiagnosticReport(DiagnosticReport report);

		/// <summary>
		/// Creates a security policy.
		/// </summary>
		/// <param name="policy">The security policy to be created.</param>
		/// <returns>Returns the newly created security policy.</returns>
		SecurityPolicyInfo CreatePolicy(SecurityPolicyInfo policy);

		/// <summary>
		/// Creates a security role.
		/// </summary>
		/// <param name="role">The security role to be created.</param>
		/// <returns>Returns the newly created security role.</returns>
		SecurityRoleInfo CreateRole(SecurityRoleInfo role);

		/// <summary>
		/// Creates a security user.
		/// </summary>
		/// <param name="user">The security user to be created.</param>
		/// <returns>Returns the newly created security user.</returns>
		SecurityUserInfo CreateUser(SecurityUserInfo user);

		/// <summary>
		/// Deletes an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be deleted.</param>
		/// <returns>Returns the deleted applet.</returns>
		AppletManifestInfo DeleteApplet(string appletId);

		/// <summary>
		/// Deletes an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be deleted.</param>
		/// <returns>Returns the deleted application.</returns>
		SecurityApplicationInfo DeleteApplication(string applicationId);

		/// <summary>
		/// Deletes an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be deleted.</param>
		/// <returns>Returns the deleted assigning authority.</returns>
		AssigningAuthorityInfo DeleteAssigningAuthority(string assigningAuthorityId);

		/// <summary>
		/// Deletes a specified certificate.
		/// </summary>
		/// <param name="id">The id of the certificate to be deleted.</param>
		/// <param name="reason">The reason the certificate is to be deleted.</param>
		/// <returns>Returns the deletion result.</returns>
		SubmissionResult DeleteCertificate(string id, RevokeReason reason);

		/// <summary>
		/// Deletes a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be deleted.</param>
		/// <returns>Returns the deleted device.</returns>
		SecurityDeviceInfo DeleteDevice(string deviceId);

		/// <summary>
		/// Deletes a security policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be deleted.</param>
		/// <returns>Returns the deleted policy.</returns>
		SecurityPolicyInfo DeletePolicy(string policyId);

		/// <summary>
		/// Deletes a security role.
		/// </summary>
		/// <param name="roleId">The id of the role to be deleted.</param>
		/// <returns>Returns the deleted role.</returns>
		SecurityRoleInfo DeleteRole(string roleId);

		/// <summary>
		/// Deletes a security user.
		/// </summary>
		/// <param name="userId">The id of the user to be deleted.</param>
		/// <returns>Returns the deleted user.</returns>
		SecurityUserInfo DeleteUser(string userId);

		/// <summary>
		/// Gets a specific alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to retrieve.</param>
		/// <returns>Returns the alert.</returns>
		AlertMessageInfo GetAlert(string alertId);

		/// <summary>
		/// Gets a list of alert for a specific query.
		/// </summary>
		/// <returns>Returns a list of alert which match the specific query.</returns>
		AmiCollection<AlertMessageInfo> GetAlerts();

		/// <summary>
		/// Gets a specific applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to retrieve.</param>
		/// <returns>Returns the applet.</returns>
		AppletManifestInfo GetApplet(string appletId);

		/// <summary>
		/// Gets a list of applets for a specific query.
		/// </summary>
		/// <returns>Returns a list of applet which match the specific query.</returns>
		AmiCollection<AppletManifestInfo> GetApplets();

		/// <summary>
		/// Gets a specific application.
		/// </summary>
		/// <param name="applicationId">The id of the application to retrieve.</param>
		/// <returns>Returns the application.</returns>
		SecurityApplicationInfo GetApplication(string applicationId);

		/// <summary>
		/// Gets a list applications for a specific query.
		/// </summary>
		/// <returns>Returns a list of application which match the specific query.</returns>
		AmiCollection<SecurityApplicationInfo> GetApplications();

		/// <summary>
		/// Gets a list of assigning authorities for a specific query.
		/// </summary>
		/// <returns>Returns a list of assigning authorities which match the specific query.</returns>
		AmiCollection<AssigningAuthorityInfo> GetAssigningAuthorities();

		/// <summary>
		/// Gets a specific assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to retrieve.</param>
		/// <returns>Returns the assigning authority.</returns>
		AssigningAuthorityInfo GetAssigningAuthority(string assigningAuthorityId);

		/// <summary>
		/// Gets a specific certificate.
		/// </summary>
		/// <param name="id">The id of the certificate to retrieve.</param>
		/// <returns>Returns the certificate.</returns>
		byte[] GetCertificate(string id);

		/// <summary>
		/// Gets a list of certificates.
		/// </summary>
		/// <returns>Returns a list of certificates.</returns>
		AmiCollection<X509Certificate2Info> GetCertificates();

		/// <summary>
		/// Gets the certificate revocation list.
		/// </summary>
		/// <returns>Returns the certificate revocation list.</returns>
		byte[] GetCrl();

		/// <summary>
		/// Gets a specific certificate signing request.
		/// </summary>
		/// <param name="id">The id of the certificate signing request to be retrieved.</param>
		/// <returns>Returns the certificate signing request.</returns>
		SubmissionResult GetCsr(string id);

		/// <summary>
		/// Gets a list of submitted certificate signing requests.
		/// </summary>
		/// <returns>Returns a list of certificate signing requests.</returns>
		AmiCollection<SubmissionInfo> GetCsrs();

		/// <summary>
		/// Gets a specific device.
		/// </summary>
		/// <param name="deviceId">The id of the security device to be retrieved.</param>
		/// <returns>Returns the security device.</returns>
		SecurityDeviceInfo GetDevice(string deviceId);

		/// <summary>
		/// Gets a list of devices.
		/// </summary>
		/// <returns>Returns a list of devices.</returns>
		AmiCollection<SecurityDeviceInfo> GetDevices();

		/// <summary>
		/// Gets a list of policies.
		/// </summary>
		/// <returns>Returns a list of policies.</returns>
		AmiCollection<SecurityPolicyInfo> GetPolicies();

		/// <summary>
		/// Gets a specific security policy.
		/// </summary>
		/// <param name="policyId">The id of the security policy to be retrieved.</param>
		/// <returns>Returns the security policy.</returns>
		SecurityPolicyInfo GetPolicy(string policyId);

		/// <summary>
		/// Gets a specific security role.
		/// </summary>
		/// <param name="roleId">The id of the security role to be retrieved.</param>
		/// <returns>Returns the security role.</returns>
		SecurityRoleInfo GetRole(string roleId);

		/// <summary>
		/// Gets a list of security roles.
		/// </summary>
		/// <returns>Returns a list of security roles.</returns>
		AmiCollection<SecurityRoleInfo> GetRoles();

		/// <summary>
		/// Gets the schema for the administrative interface.
		/// </summary>
		/// <param name="schemaId">The id of the schema to be retrieved.</param>
		/// <returns>Returns the administrative interface schema.</returns>
		XmlSchema GetSchema(int schemaId);

		/// <summary>
		/// Gets the list of TFA mechanisms.
		/// </summary>
		/// <returns>Returns a list of TFA mechanisms.</returns>
		AmiCollection<TfaMechanismInfo> GetTfaMechanisms();

		/// <summary>
		/// Gets a specific security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be retrieved.</param>
		/// <returns>Returns the security user.</returns>
		SecurityUserInfo GetUser(string userId);

		/// <summary>
		/// Gets a list of security users.
		/// </summary>
		/// <returns>Returns a list of security users.</returns>
		AmiCollection<SecurityUserInfo> GetUsers();

		/// <summary>
		/// Gets options for the AMI service.
		/// </summary>
		/// <returns>Returns options for the AMI service.</returns>
		IdentifiedData Options();

		/// <summary>
		/// Rejects a specified certificate signing request.
		/// </summary>
		/// <param name="certId">The id of the certificate signing request to be rejected.</param>
		/// <param name="reason">The reason the certificate signing request is to be rejected.</param>
		/// <returns>Returns the rejection result.</returns>
		SubmissionResult RejectCsr(string certId, RevokeReason reason);

		/// <summary>
		/// Creates a request that the server issue a reset code
		/// </summary>
		void SendTfaSecret(TfaRequestInfo resetInfo);

		/// <summary>
		/// Submits a specific certificate signing request.
		/// </summary>
		/// <param name="s">The certificate signing request.</param>
		/// <returns>Returns the submission result.</returns>
		SubmissionResult SubmitCsr(SubmissionRequest s);

		/// <summary>
		/// Updates an alert.
		/// </summary>
		/// <param name="alertId">The id of the alert to be updated.</param>
		/// <param name="alert">The alert containing the updated information.</param>
		/// <returns>Returns the updated alert.</returns>
		AlertMessageInfo UpdateAlert(string alertId, AlertMessageInfo alert);

		/// <summary>
		/// Updates an applet.
		/// </summary>
		/// <param name="appletId">The id of the applet to be updated.</param>
		/// <param name="appletManifestInfo">The applet containing the updated information.</param>
		/// <returns>Returns the updated applet.</returns>
		AppletManifestInfo UpdateApplet(string appletId, AppletManifestInfo appletManifestInfo);

		/// <summary>
		/// Updates an application.
		/// </summary>
		/// <param name="applicationId">The id of the application to be updated.</param>
		/// <param name="applicationInfo">The application containing the updated information.</param>
		/// <returns>Returns the updated application.</returns>
		SecurityApplicationInfo UpdateApplication(string applicationId, SecurityApplicationInfo applicationInfo);

		/// <summary>
		/// Updates an assigning authority.
		/// </summary>
		/// <param name="assigningAuthorityId">The id of the assigning authority to be updated.</param>
		/// <param name="assigningAuthorityInfo">The assigning authority containing the updated information.</param>
		/// <returns>Returns the updated assigning authority.</returns>
		AssigningAuthorityInfo UpdateAssigningAuthority(string assigningAuthorityId, AssigningAuthorityInfo assigningAuthorityInfo);

		/// <summary>
		/// Updates a device.
		/// </summary>
		/// <param name="deviceId">The id of the device to be updated.</param>
		/// <param name="deviceInfo">The device containing the updated information.</param>
		/// <returns>Returns the updated device.</returns>
		SecurityDeviceInfo UpdateDevice(string deviceId, SecurityDeviceInfo deviceInfo);

		/// <summary>
		/// Updates a policy.
		/// </summary>
		/// <param name="policyId">The id of the policy to be updated.</param>
		/// <param name="policyInfo">The policy containing the updated information.</param>
		/// <returns>Returns the updated policy.</returns>
		SecurityPolicyInfo UpdatePolicy(string policyId, SecurityPolicyInfo policyInfo);

		/// <summary>
		/// Updates a role.
		/// </summary>
		/// <param name="roleId">The id of the role to be updated.</param>
		/// <param name="roleInfo">The role containing the updated information.</param>
		/// <returns>Returns the updated role.</returns>
		SecurityRoleInfo UpdateRole(string roleId, SecurityRoleInfo roleInfo);

		/// <summary>
		/// Updates a security user.
		/// </summary>
		/// <param name="userId">The id of the security user to be retrieved.</param>
		/// <param name="userInfo">The user containing the updated information.</param>
		/// <returns>Returns the security user.</returns>
		SecurityUserInfo UpdateUser(string userId, SecurityUserInfo userInfo);
	}
}
