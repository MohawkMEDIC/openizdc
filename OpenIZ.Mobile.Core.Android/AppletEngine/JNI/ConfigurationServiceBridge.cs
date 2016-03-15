using System;
using Android.Webkit;
using Java.Interop;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Android.Security;
using System.Security.Permissions;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Android.Http;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	/// <summary>
	/// Represents an administrative functions bridge which controls the application itself
	/// </summary>
	public class ConfigurationServiceBridge : Java.Lang.Object
	{

		private Tracer m_tracer = Tracer.GetTracer (typeof(ConfigurationServiceBridge));
		/// <summary>
		/// Backs up the database to the user's SD card
		/// </summary>
		[Export]
		[JavascriptInterface]
		public bool Save(String options)
		{
			// Demand the appropriate policy
			new PolicyPermission(PermissionState.Unrestricted, PolicyIdentifiers.AccessClientAdministrativeFunction).Demand();

			this.m_tracer.TraceInfo ("Saving configuration options {0}", options);
			AndroidApplicationContext.Current.ConfigurationManager.Save ();
			return true;
		}

		/// <summary>
		/// Joins the specified realm
		/// </summary>
		[Export]
		[JavascriptInterface]
		public bool JoinRealm(String realmUri)
		{
			this.m_tracer.TraceInfo ("Joining {0}", realmUri);

			// Demand policy
			try
			{
				new PolicyPermission(PermissionState.Unrestricted, PolicyIdentifiers.AccessAdministrativeFunction).Demand();

				// Configure the stuff to the appropriate realm info
				var serviceClientSection = AndroidApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>();
				if(serviceClientSection == null)
				{
					serviceClientSection = new ServiceClientConfigurationSection() {
						RestClientType = typeof(RestClient)
					};
					AndroidApplicationContext.Current.Configuration.Sections.Add(serviceClientSection);
				}

				// TODO: Actually contact the AMI for this information

				// Parse URI
				Uri baseUri = new Uri(realmUri);
				String imsiUri = String.Format("http://{0}:{1}/imsi", baseUri.Host, baseUri.Port),
					oauthUri = String.Format("http://{0}:{1}/auth", baseUri.Host, baseUri.Port);

				// Parse IMSI URI
				serviceClientSection.Client.Add(new ServiceClient() {
					Binding = new ServiceClientBinding() {
						Security = new ServiceClientSecurity() {
							AuthRealm = realmUri,
							Mode = SecurityScheme.Bearer,
							CredentialProvider = new TokenCredentialProvider()
						}
					},
					Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>() {
						new ServiceClientEndpoint() {
							Address = imsiUri
						}
					},
					Name = "acs"
				});

				// Parse ACS URI
				serviceClientSection.Client.Add(new ServiceClient() {
					Binding = new ServiceClientBinding() {
						Security = new ServiceClientSecurity() {
							AuthRealm = realmUri,
							Mode = SecurityScheme.Basic,
							CredentialProvider = new OAuth2CredentialProvider()
						}
					},
					Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>() {
						new ServiceClientEndpoint() {
							Address = oauthUri
						}
					},
					Name = "acs"
				});

			}
			catch(PolicyViolationException e)
			{
				// TODO: Login permission
			}
			catch(Exception e)
			{
				this.m_tracer.TraceError("Error joining context: {0}", e);

			}
			return true;
		}
	}
}

