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
 * Date: 2016-6-14
 */
using System;
using Android.Webkit;
using Java.Interop;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Xamarin.Security;
using System.Security.Permissions;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin.Http;
using OpenIZ.Core.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIZ.Mobile.Core.Data;
using System.Diagnostics.Tracing;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Interop.IMSI;
using OpenIZ.Mobile.Core.Android.Resources;
using OpenIZ.Mobile.Core.Alerting;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Mobile.Core.Xamarin.Diagnostics;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
    /// <summary>
    /// Represents an administrative functions bridge which controls the application itself
    /// </summary>
    public class ConfigurationServiceBridge : Java.Lang.Object
    {
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ConfigurationServiceBridge));

        /// <summary>
        /// Gets the specified value for the specified app setting key
        /// </summary>
        [Export]
        [JavascriptInterface]
        public String GetApplicationSetting(String key)
        {
            try
            {
                return ApplicationContext.Current.Configuration.GetAppSetting(key);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error getting setting {0} : {1}", key, e);
                return null;
            }
        }

        /// <summary>
        /// Gets the specified section name
        /// </summary>
        /// <returns>The section.</returns>
        /// <param name="sectionName">Section name.</param>
        [Export]
        [JavascriptInterface]
        public String GetSection(String sectionName)
        {
            Type sectionType = Type.GetType(String.Format("OpenIZ.Mobile.Core.Configuration.{0}, OpenIZ.Mobile.Core, Version=0.1.0.0", sectionName));
            if (sectionType == null)
                return null;
            else
            {
                return JniUtil.ToJson(ApplicationContext.Current.Configuration.GetSection(sectionType));
            }
        }

        /// <summary>
        /// Backs up the database to the user's SD card
        /// </summary>
        [Export]
        [JavascriptInterface]
        public bool Save(String options)
        {
            // Demand the appropriate policy
            new PolicyPermission(PermissionState.Unrestricted, PolicyIdentifiers.AccessClientAdministrativeFunction).Demand();

            JObject optionObject = JsonConvert.DeserializeObject(options) as JObject;

            // Data mode
            switch (optionObject["data"]["mode"].Value<String>())
            {
                case "online":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(LocalPolicyInformationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiPolicyInformationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(OAuthIdentityProvider).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(ImsiPersistenceService).AssemblyQualifiedName);
                    break;
                case "offline":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalPersistenceService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalIdentityService).AssemblyQualifiedName);
                    break;
                case "sync":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalPersistenceService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(OAuthIdentityProvider).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(QueueManagerService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(RemoteSynchronizationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(ImsiIntegrationService).AssemblyQualifiedName);

                    // Sync settings
                    var syncConfig = new SynchronizationConfigurationSection();
                    // TODO: Customize this
                    foreach(var res in new String[] { "ConceptSet", "AssigningAuthority", "IdentifierType", "ExtensionType", "ConceptClass", "Concept", "Material", "Place", "Organization", "SecurityRole", "UserEntity", "Provider", "ManufacturedMaterial"  })
                    {
                        var syncSetting = new SynchronizationResource()
                        {
                            ResourceAqn = res,
                            Triggers = SynchronizationPullTriggerType.Always
                        };

                        var efield = typeof(EntityClassKeys).GetField(res);
                        if(efield != null && res != "Place")
                            syncSetting.Filters.Add("classConcept=" + efield.GetValue(null).ToString());

                        syncConfig.SynchronizationResources.Add(syncSetting);
                    }
                    ApplicationContext.Current.Configuration.Sections.Add(syncConfig);
                    break;
            }
            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalRoleProviderService).AssemblyQualifiedName);
            // Password hashing
            switch (optionObject["security"]["hasher"].Value<String>())
            {
                case "SHA256PasswordHasher":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(SHA256PasswordHasher).AssemblyQualifiedName);
                    break;
                case "SHAPasswordHasher":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(SHAPasswordHasher).AssemblyQualifiedName);
                    break;
                case "PlainTextPasswordHasher":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(PlainTextPasswordHasher).AssemblyQualifiedName);
                    break;
            }

            // Proxy
            if(optionObject["network"]["useProxy"].Value<Boolean>())
                ApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>().ProxyAddress = optionObject["network"]["proxyAddress"].Value<String>();
            
            // Log settings
            var logSettings = ApplicationContext.Current.Configuration.GetSection<DiagnosticsConfigurationSection>();
            logSettings.TraceWriter = new System.Collections.Generic.List<TraceWriterConfiguration>()
            {
#if DEBUG
                new TraceWriterConfiguration () {
                        Filter = System.Diagnostics.Tracing.EventLevel.LogAlways,
                        InitializationData = "OpenIZ",
                        TraceWriter = new LogTraceWriter (System.Diagnostics.Tracing.EventLevel.LogAlways, "OpenIZ")
                    },
#endif
                new TraceWriterConfiguration()
                {
                    Filter = (EventLevel)Enum.Parse(typeof(EventLevel), optionObject["log"]["mode"].Value<String>()),
                    InitializationData = "OpenIZ",
                    TraceWriter = new FileTraceWriter((EventLevel)Enum.Parse(typeof(EventLevel), optionObject["log"]["mode"].Value<String>()), "OpenIZ")

                }

            };

            this.m_tracer.TraceInfo("Saving configuration options {0}", options);
            AndroidApplicationContext.Current.ConfigurationManager.Save();

           
            return true;
        }

        /// <summary>
        /// Joins the specified realm
        /// </summary>
        [Export]
        [JavascriptInterface]
        public bool JoinRealm(String realmUri, String deviceName)
        {
            this.m_tracer.TraceInfo("Joining {0}", realmUri);

            // Demand policy
            try
            {
                if (AndroidApplicationContext.Current.ConfigurationManager.IsConfigured)
                    new PolicyPermission(PermissionState.Unrestricted, PolicyIdentifiers.UnrestrictedAdministration).Demand();


                // Configure the stuff to the appropriate realm info
                var serviceClientSection = AndroidApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>();
                if (serviceClientSection == null)
                {
                    serviceClientSection = new ServiceClientConfigurationSection()
                    {
                        RestClientType = typeof(RestClient)
                    };
                    AndroidApplicationContext.Current.Configuration.Sections.Add(serviceClientSection);
                }
                else
                    serviceClientSection.Client.Clear();

                // TODO: Actually contact the AMI for this information


                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().DeviceName = deviceName;
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().Domain = realmUri;
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().TokenAlgorithms = new System.Collections.Generic.List<string>() {
                    "RS256",
                    "HS256"
                };
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().TokenType = "urn:ietf:params:oauth:token-type:jwt";


                String imsiUri = String.Format("http://{0}:8080/imsi", realmUri),
                    oauthUri = String.Format("http://{0}:8080/auth", realmUri),
                    amiUri = String.Format("http://{0}:8080/ami", realmUri);

                // Parse IMSI URI
                serviceClientSection.Client.Add(new ServiceClientDescription()
                {
                    Binding = new ServiceClientBinding()
                    {
                        Security = new ServiceClientSecurity()
                        {
                            AuthRealm = realmUri,
                            Mode = SecurityScheme.Bearer,
                            CredentialProvider = new TokenCredentialProvider(),
                            PreemptiveAuthentication = true
                        },
                        Optimize = true
                    },
                    Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>() {
                        new ServiceClientEndpoint() {
                            Address = imsiUri
                        }
                    },
                    Name = "imsi"
                });

                // Parse ACS URI
                serviceClientSection.Client.Add(new ServiceClientDescription()
                {
                    Binding = new ServiceClientBinding()
                    {
                        Security = new ServiceClientSecurity()
                        {
                            AuthRealm = realmUri,
                            Mode = SecurityScheme.Bearer,
                            CredentialProvider = new TokenCredentialProvider(),
                            PreemptiveAuthentication = true
                        },
                        Optimize = false
                    },
                    Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>() {
                        new ServiceClientEndpoint() {
                            Address = amiUri
                        }
                    },
                    Name = "ami"
                });

                // Parse ACS URI
                serviceClientSection.Client.Add(new ServiceClientDescription()
                {
                    Binding = new ServiceClientBinding()
                    {
                        Security = new ServiceClientSecurity()
                        {
                            AuthRealm = realmUri,
                            Mode = SecurityScheme.Basic,
                            CredentialProvider = new OAuth2CredentialProvider()
                        },
                        Optimize = false
                    },
                    Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>() {
                        new ServiceClientEndpoint() {
                            Address = oauthUri
                        }
                    },
                    Name = "acs"
                });

            }
            catch (PolicyViolationException e)
            {
                // TODO: Login permission

            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error joining context: {0}", e);

            }
            return true;
        }
    }
}

