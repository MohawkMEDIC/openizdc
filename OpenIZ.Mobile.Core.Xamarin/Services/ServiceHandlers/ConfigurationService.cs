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
 * Date: 2016-10-11
 */
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenIZ.Core.Http.Description;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Query;
using OpenIZ.Messaging.AMI.Client;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Data;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Exceptions;
using OpenIZ.Mobile.Core.Interop.IMSI;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Synchronization;
using OpenIZ.Mobile.Core.Xamarin.Diagnostics;
using OpenIZ.Mobile.Core.Xamarin.Http;
using OpenIZ.Mobile.Core.Xamarin.Security;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using OpenIZ.Mobile.Core.Interop;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Entities;
using System.Data;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using OpenIZ.Mobile.Core.Security.Remote;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Configuration view model
    /// </summary>
    [JsonObject]
    public class ConfigurationViewModel
    {
        public ConfigurationViewModel()
        {

        }

        /// <summary>
        /// Configuation
        /// </summary>
        /// <param name="config"></param>
        public ConfigurationViewModel(OpenIZConfiguration config)
        {
            this.RealmName = config.GetSection<SecurityConfigurationSection>().Domain;
            this.Security = config.GetSection<SecurityConfigurationSection>();
            this.Data = config.GetSection<DataConfigurationSection>();
            this.Applet = config.GetSection<AppletConfigurationSection>();
            this.Application = config.GetSection<ApplicationConfigurationSection>();
            this.Log = config.GetSection<DiagnosticsConfigurationSection>();
            this.Network = config.GetSection<ServiceClientConfigurationSection>();
        }
        /// <summary>
        /// Security section
        /// </summary>
        [JsonProperty("security")]
        public SecurityConfigurationSection Security { get; set; }
        /// <summary>
        /// Realm name
        /// </summary>
        [JsonProperty("realmName")]
        public String RealmName { get; set; }
        /// <summary>
        /// Data config
        /// </summary>
        [JsonProperty("data")]
        public DataConfigurationSection Data { get; set; }
        /// <summary>
        /// Gets or sets applet
        /// </summary>
        [JsonProperty("applet")]
        public AppletConfigurationSection Applet { get; set; }
        /// <summary>
        /// Gets or sets application
        /// </summary>
        [JsonProperty("application")]
        public ApplicationConfigurationSection Application { get; set; }
        /// <summary>
        /// Log
        /// </summary>
        [JsonProperty("log")]
        public DiagnosticsConfigurationSection Log { get; set; }
        /// <summary>
        /// Gets or sets the network
        /// </summary>
        [JsonProperty("network")]
        public ServiceClientConfigurationSection Network { get; set; }

    }

    /// <summary>
    /// Restful service
    /// </summary>
    [RestService("/__config")]
    public class ConfigurationService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ConfigurationService));

        /// <summary>
        /// Gets the specified forecast
        /// </summary>
        [RestOperation(UriPath = "/all", Method = "GET", FaultProvider = nameof(ConfigurationFaultProvider))]
        [return: RestMessage(RestMessageFormat.Json)]
        public ConfigurationViewModel GetConfiguration()
        {
            return new ConfigurationViewModel(XamarinApplicationContext.Current.Configuration);
        }

        /// <summary>
        /// Save configuration
        /// </summary>
        [RestOperation(UriPath = "/all", Method = "POST", FaultProvider = nameof(ConfigurationFaultProvider))]
        [Demand(PolicyIdentifiers.AccessClientAdministrativeFunction)]
        [return: RestMessage(RestMessageFormat.Json)]
        public ConfigurationViewModel SaveConfiguration([RestMessage(RestMessageFormat.Json)]JObject optionObject)
        {
            // Clean up join realm stuff
            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(OAuthIdentityProvider).AssemblyQualifiedName);
            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(AmiPolicyInformationService).AssemblyQualifiedName);
            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(ImsiPersistenceService).AssemblyQualifiedName);

            // Data mode
            switch (optionObject["data"]["mode"].Value<String>())
            {
                case "online":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(LocalPolicyInformationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiPolicyInformationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(OAuthIdentityProvider).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(ImsiPersistenceService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiTwoFactorRequestService).AssemblyQualifiedName);

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
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiTwoFactorRequestService).AssemblyQualifiedName);

                    // Sync settings
                    var syncConfig = new SynchronizationConfigurationSection();
                    // TODO: Customize this
                    foreach (var res in new String[] { "ConceptSet", "AssigningAuthority", "IdentifierType", "ConceptClass", "Concept", "Material", "Place", "Organization", "SecurityRole", "UserEntity", "Provider", "ManufacturedMaterial" })
                    {
                        var syncSetting = new SynchronizationResource()
                        {
                            ResourceAqn = res,
                            Triggers = SynchronizationPullTriggerType.Always
                        };

                        var efield = typeof(EntityClassKeys).GetField(res);
                        if (efield != null && res != "Place")
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
            if (optionObject["network"]["useProxy"].Value<Boolean>())
                ApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>().ProxyAddress = optionObject["network"]["proxyAddress"].Value<String>();

            // Log settings
            var logSettings = ApplicationContext.Current.Configuration.GetSection<DiagnosticsConfigurationSection>();
            logSettings.TraceWriter = new System.Collections.Generic.List<TraceWriterConfiguration>()
            {
#if DEBUG
                new TraceWriterConfiguration () {
                        Filter = System.Diagnostics.Tracing.EventLevel.Critical,
                        InitializationData = "OpenIZ",
                        TraceWriter = new LogTraceWriter (System.Diagnostics.Tracing.EventLevel.Critical, "OpenIZ")
                    },
#endif
                new TraceWriterConfiguration()
                {
                    Filter = (EventLevel)Enum.Parse(typeof(EventLevel), optionObject["log"]["mode"].Value<String>()),
                    InitializationData = "OpenIZ",
                    TraceWriter = new FileTraceWriter((EventLevel)Enum.Parse(typeof(EventLevel), optionObject["log"]["mode"].Value<String>()), "OpenIZ")

                }

            };

            this.m_tracer.TraceInfo("Saving configuration options {0}", optionObject);
            XamarinApplicationContext.Current.ConfigurationManager.Save();
            
            return new ConfigurationViewModel(XamarinApplicationContext.Current.Configuration) ;
        }

        /// <summary>
        /// Join a realm
        /// </summary>
        /// <param name="realmData"></param>
        [RestOperation(UriPath = "/realm", Method = "POST", FaultProvider = nameof(ConfigurationFaultProvider))]
        [Demand(PolicyIdentifiers.AccessClientAdministrativeFunction)]
        [return: RestMessage(RestMessageFormat.Json)]
        public ConfigurationViewModel JoinRealm([RestMessage(RestMessageFormat.FormData)]NameValueCollection realmData)
        {
            String realmUri = realmData["realmUri"][0];
            String deviceName = realmData["deviceName"][0];
            this.m_tracer.TraceInfo("Joining {0}", realmUri);

            // Stage 1 - Demand access admin policy
            try
            {

                new PolicyPermission(PermissionState.Unrestricted, PolicyIdentifiers.UnrestrictedAdministration).Demand();

                // We're allowed to access server admin!!!! Yay!!!
                // We're goin to conigure the realm settings now (all of them)
                var serviceClientSection = XamarinApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>();
                if (serviceClientSection == null)
                {
                    serviceClientSection = new ServiceClientConfigurationSection()
                    {
                        RestClientType = typeof(RestClient)
                    };
                    XamarinApplicationContext.Current.Configuration.Sections.Add(serviceClientSection);
                }

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
                        Optimize = true
                    },
                    Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>() {
                        new ServiceClientEndpoint() {
                            Address = amiUri, Timeout = 10000
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

                ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new AmiPolicyInformationService());
                ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new ImsiPersistenceService());
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().DeviceSecret = Guid.NewGuid().ToString().Replace("-", "");

                // Create the necessary device user
                try
                {
                    AmiServiceClient amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));
                    // Create application user
                    var role = amiClient.GetRoles(o=>o.Name == "SYNCHRONIZERS").CollectionItem.First();


                    // Does the user actually exist?
                    var existingClient = amiClient.GetUsers(o => o.UserName == deviceName);
                    if (existingClient.CollectionItem.Count > 0)
                    {
                        if (!realmData.ContainsKey("force") || !Boolean.Parse(realmData["force"][0]))
                            throw new DuplicateNameException(Strings.err_duplicate_deviceName);
                        else
                            amiClient.UpdateUser(existingClient.CollectionItem.First().UserId.Value, new OpenIZ.Core.Model.AMI.Auth.SecurityUserInfo()
                            {
                                Password = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().DeviceSecret,
                                UserName = deviceName
                            });
                    }
                    else
                        // Create user
                        amiClient.CreateUser(new OpenIZ.Core.Model.AMI.Auth.SecurityUserInfo(new OpenIZ.Core.Model.Security.SecurityUser()
                        {
                            UserName = deviceName,
                            Key = Guid.NewGuid(),
                            UserClass = UserClassKeys.ApplictionUser, 
                            SecurityHash = Guid.NewGuid().ToString()
                        })
                        {
                            Roles = new List<OpenIZ.Core.Model.AMI.Auth.SecurityRoleInfo>()
                            {
                                role
                            },
                            Password = ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().DeviceSecret,
                        });

                    // TODO: Generate the CSR
                    
                    // Lookup sync role
                    var existingDevice = amiClient.GetDevices(o => o.Name == deviceName);
                    if (existingDevice.CollectionItem.Count == 0)
                    {
                        // Create device
                        var newDevice = amiClient.CreateDevice(new OpenIZ.Core.Model.Security.SecurityDevice()
                        {
                            Name = deviceName,
                            DeviceSecret = Guid.NewGuid().ToString()
                        });

                        //// Now create device entity
                        //var newDeviceEntity = ApplicationContext.Current.GetService<IDataPersistenceService<DeviceEntity>>().Insert(new DeviceEntity()
                        //{
                        //    SecurityDevice = newDevice,
                        //    StatusConceptKey = StatusKeys.Active,
                        //    ManufacturedModelName = Environment.MachineName,
                        //    OperatingSystemName = Environment.OSVersion.ToString(),
                        //});
                    }
                    else
                        ; // TODO: Update
                }
                catch(Exception e)
                {
                    this.m_tracer.TraceError("Error registering device account: {0}", e);
                    throw;
                }
                return new ConfigurationViewModel(XamarinApplicationContext.Current.Configuration);
            }
            catch(PolicyViolationException ex)
            {
                this.m_tracer.TraceWarning("Policy violation exception on {0}. Will attempt again", ex.Demanded);
                // Only configure the minimum to contact the realm for authentication to continue
                var serviceClientSection = XamarinApplicationContext.Current.Configuration.GetSection<ServiceClientConfigurationSection>();
                if (serviceClientSection == null)
                {
                    serviceClientSection = new ServiceClientConfigurationSection()
                    {
                        RestClientType = typeof(RestClient)
                    };
                    XamarinApplicationContext.Current.Configuration.Sections.Add(serviceClientSection);
                }

                // TODO: Actually contact the AMI for this information
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().DeviceName = deviceName;
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().Domain = realmUri;
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().TokenAlgorithms = new System.Collections.Generic.List<string>() {
                    "RS256",
                    "HS256"
                };
                
                String oauthUri = String.Format("http://{0}:8080/auth", realmUri),
                                        amiUri = String.Format("http://{0}:8080/ami", realmUri);

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
                            Address = amiUri, Timeout = 10000
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

                ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new OAuthIdentityProvider());

                throw new UnauthorizedAccessException();
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error joining context: {0}", e);
                throw;
            }
            return null;
        }

        /// <summary>
        /// Handle a fault
        /// </summary>
        public ErrorResult ConfigurationFaultProvider(Exception e)
        {
            return new ErrorResult()
            {
                Error = e.Message,
                ErrorDescription = e.InnerException?.Message,
                ErrorType = e.GetType().Name
            };
        }
    }
}
