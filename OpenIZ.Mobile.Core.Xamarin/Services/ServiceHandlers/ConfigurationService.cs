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
 * User: justi
 * Date: 2017-3-31
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
using OpenIZ.Mobile.Core.Data.Connection;
using OpenIZ.Core.Model.AMI.Auth;
using OpenIZ.Core.Model;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Caching;
using OpenIZ.Mobile.Core.Alerting;
using OpenIZ.Mobile.Core.Interop.AMI;
using OpenIZ.Mobile.Core.Security.Audit;
using System.Net;
using OpenIZ.Core.Interop;
using OpenIZ.Core.Http;

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
            if (config == null) return;

            this.RealmName = config.GetSection<SecurityConfigurationSection>()?.Domain;
            this.Security = config.GetSection<SecurityConfigurationSection>();
            this.Data = config.GetSection<DataConfigurationSection>();
            this.Applet = config.GetSection<AppletConfigurationSection>();
            this.Application = config.GetSection<ApplicationConfigurationSection>();
            this.Log = config.GetSection<DiagnosticsConfigurationSection>();
            this.Network = config.GetSection<ServiceClientConfigurationSection>();
            this.Synchronization = config.GetSection<SynchronizationConfigurationSection>();
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
        [JsonProperty("sync")]
        public SynchronizationConfigurationSection Synchronization { get; set; }
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
        /// Gets the currently authenticated user's configuration
        /// </summary>
        [RestOperation(UriPath = "/user", Method = "GET", FaultProvider = nameof(ConfigurationFaultProvider))]
        [return: RestMessage(RestMessageFormat.Json)]
        [Demand(PolicyIdentifiers.Login)]
        public ConfigurationViewModel GetUserConfiguration()
        {
            String userId = MiniImsServer.CurrentContext.Request.QueryString["_id"] ?? AuthenticationContext.Current.Principal.Identity.Name;
            return new ConfigurationViewModel(XamarinApplicationContext.Current.GetUserConfiguration(userId));

        }

        /// <summary>
        /// Gets the currently authenticated user's configuration
        /// </summary>
        [RestOperation(UriPath = "/user", Method = "POST", FaultProvider = nameof(ConfigurationFaultProvider))]
        public void SaveUserConfiguration([RestMessage(RestMessageFormat.Json)]ConfigurationViewModel model)
        {
            String userId = MiniImsServer.CurrentContext.Request.QueryString["_id"] ?? AuthenticationContext.Current.Principal.Identity.Name;
            XamarinApplicationContext.Current.SaveUserConfiguration(userId,
                new OpenIZConfiguration()
                {
                    Sections = new List<object>()
                    {
                        model.Application,
                        model.Applet
                    }
                }
            );
        }

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
            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(AmiPolicyInformationService).AssemblyQualifiedName);
            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(ImsiPersistenceService).AssemblyQualifiedName);
            ApplicationContext.Current.Configuration.Sections.RemoveAll(o => o is SynchronizationConfigurationSection);

            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalAuditRepositoryService).AssemblyQualifiedName);
            ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalAuditService).AssemblyQualifiedName);

            // Data mode
            switch (optionObject["data"]["mode"].Value<String>())
            {
                case "online":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(LocalPolicyInformationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiPolicyInformationService).AssemblyQualifiedName);

                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(ImsiPersistenceService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiTwoFactorRequestService).AssemblyQualifiedName);

                    break;
                case "offline":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.RemoveAll(o => o == typeof(OAuthIdentityProvider).AssemblyQualifiedName || o == typeof(HttpBasicIdentityProvider).AssemblyQualifiedName);

                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalIdentityService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Insert(0, typeof(SQLiteConnectionManager).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalPersistenceService).AssemblyQualifiedName);
                    break;
                case "sync":
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Insert(0, typeof(SQLiteConnectionManager).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalPersistenceService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(LocalAlertService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(QueueManagerService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(RemoteSynchronizationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(ImsiIntegrationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiIntegrationService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AmiTwoFactorRequestService).AssemblyQualifiedName);
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(AlertSynchronizationService).AssemblyQualifiedName);

                    // Sync settings
                    var syncConfig = new SynchronizationConfigurationSection();
                    var binder = new OpenIZ.Core.Model.Serialization.ModelSerializationBinder();
                    // TODO: Customize this and clean it up ... It is very hackish
                    foreach (var res in new String[] {
                        "ConceptSet",
                        "AssigningAuthority",
                        "IdentifierType",
                        "TemplateDefinition",
                        "ExtensionType",
                        "ConceptClass",
                        "Concept",
                        "Material",
                        "Place",
                        "PlaceMe",
                        "Organization",
                        "UserEntity",
                        "UserEntityMe",
                        "Provider",
                        "ManufacturedMaterial",
                        "Person",
                        "PatientEncounter",
                        "SubstanceAdministration",
                        "CodedObservation",
                        "QuantityObservation",
                        "TextObservation",
                        "Act"})
                    {
                        var syncSetting = new SynchronizationResource()
                        {
                            ResourceAqn = res,
                            Triggers = new String[] { "Person", "Act", "SubstanceAdministration", "QuantityObservation", "CodedObservation", "TextObservation", "PatientEncounter" }.Contains(res) ? SynchronizationPullTriggerType.Always :
                                SynchronizationPullTriggerType.OnNetworkChange | SynchronizationPullTriggerType.OnStart
                        };

                        // Subscription
                        if (optionObject["data"]["sync"]["subscribe"] == null)
                        {
                            var efield = typeof(EntityClassKeys).GetField(res);

                            if (res == "Person")
                            {
                                syncSetting.Filters.Add("classConcept=" + EntityClassKeys.Patient);
                                syncSetting.Filters.Add("classConcept=" + EntityClassKeys.Person + "&relationship.source.classConcept=" + EntityClassKeys.Patient);
                            }
                            else if (res == "Act")
                            {
                                syncSetting.Filters.Add("classConcept=" + ActClassKeys.AccountManagement);
                                syncSetting.Filters.Add("classConcept=" + ActClassKeys.Supply);
                            }
                            else if (res == "EntityRelationship" || res == "UserEntityMe") continue;
                            else if (efield != null && res != "Place")
                                syncSetting.Filters.Add("classConcept=" + efield.GetValue(null).ToString());
                        }
                        else
                        { // Only interested in a few facilities
                            foreach (var itm in optionObject["data"]["sync"]["subscribe"].Values<String>())
                            {
                                if (!syncConfig.Facilities.Contains(itm))
                                    syncConfig.Facilities.Add(itm);

                                switch (res)
                                {
                                    case "UserEntity":
                                    case "Provider":
                                        if (syncSetting.Filters.Count == 0)
                                        {
                                            syncSetting.Filters.Add("relationship[DedicatedServiceDeliveryLocation].target=!" + itm + "&_exclude=relationship&_exclude=participation");
                                            syncSetting.Filters.Add("participation.source.participation.player=" + itm + "&_exclude=relationship&_exclude=participation");
                                        }
                                        break;
                                    case "Person":
                                        syncSetting.Filters.Add("classConcept=" + EntityClassKeys.Patient + "&relationship.target=" + itm );
                                        syncSetting.Filters.Add("classConcept=" + EntityClassKeys.Person + "&relationship.source.classConcept=" + EntityClassKeys.Patient + "&relationship.source.relationship.target=" + itm );
                                        break;
                                    case "Act":
                                        syncSetting.Filters.Add("classConcept=" + ActClassKeys.Supply + "&participation[Destination].player=" + itm + "&_expand=relationship&_expand=participation");
	                                    syncSetting.Filters.Add("classConcept=" + ActClassKeys.Supply + "&participation[Location].player=" + itm + "&_expand=relationship&_expand=participation");
										syncSetting.Filters.Add("classConcept=" + ActClassKeys.AccountManagement + "&participation[Location].player=" + itm + "&_expand=relationship&_expand=participation");
                                        //syncSetting.Filters.Add("participation[EntryLocation].player=" + itm + "&_expand=relationship&_expand=participation");
                                        break;
                                    case "UserEntityMe":
                                        syncSetting.ResourceAqn = "UserEntity";
                                        syncSetting.Triggers = SynchronizationPullTriggerType.Always;
                                        syncSetting.Filters.Add("relationship[DedicatedServiceDeliveryLocation].target=" + itm + "&_expand=relationship&_expand=participation");
                                        break;
                                    case "SubstanceAdministration":
                                    case "QuantityObservation":
                                    case "CodedObservation":
                                    case "TextObservation":
                                    case "PatientEncounter":
                                        // All stuff that is happening in my facility for out of catchment
                                        syncSetting.Filters.Add("participation.player=" + itm + "&participation[RecordTarget].player.relationship.target=!" + itm);
                                        // All stuff that is happening out of my facility for any patient associated with me
                                        syncSetting.Filters.Add("participation.player=!" + itm + "&participation[RecordTarget].player.relationship.target=" + itm);
                                        //syncSetting.Filters.Add("participation[Location].player=" + itm + "&participation[RecordTarget].player.relationship[DedicatedServiceDeliveryLocation].target=!" + itm + "&_expand =relationship&_expand=participation");
                                        break;
                                    case "Place":
                                        syncSetting.Filters.Add("classConcept=" + EntityClassKeys.ServiceDeliveryLocation + "&_exclude=relationship&_exclude=participation");
                                        syncSetting.Filters.Add("classConcept=!" + EntityClassKeys.ServiceDeliveryLocation);
                                        break;
                                    case "PlaceMe":
                                        syncSetting.ResourceAqn = "Place";
                                        syncSetting.Triggers = SynchronizationPullTriggerType.Always;
                                        syncSetting.Filters.Add("id=" + itm);
                                        syncSetting.Always = true;
                                        break;
                                    case "Material":
                                        if (syncSetting.Filters.Count == 0)
                                            syncSetting.Filters.Add("classConcept=" + EntityClassKeys.Material);
                                        break;
                                    case "ManufacturedMaterial":
                                        if (syncSetting.Filters.Count == 0)
                                            syncSetting.Filters.Add("classConcept=" + EntityClassKeys.ManufacturedMaterial);
                                        break;
                                }

                            }
                        }

                        // Assignable from
                        //if (typeof(BaseEntityData).IsAssignableFrom(binder.BindToType(typeof(BaseEntityData).Assembly.FullName, res)))
                        //{
                        //    for (int i = 0; i < syncSetting.Filters.Count; i++)
                        //        syncSetting.Filters[i] += "&obsoletionTime=null";
                        //    if (syncSetting.Filters.Count == 0)
                        //        syncSetting.Filters.Add("obsoletionTime=null");
                        //}

                        // TODO: Patient registration <> facility

                        syncConfig.SynchronizationResources.Add(syncSetting);
                    }
                    syncConfig.SynchronizationResources.Add(new SynchronizationResource()
                    {
                        ResourceAqn = "EntityRelationship",
                        Triggers = SynchronizationPullTriggerType.OnCommit
                    });
                    if (optionObject["data"]["sync"]["pollInterval"].Value<String>() != "00:00:00")
                        syncConfig.PollIntervalXml = optionObject["data"]["sync"]["pollInterval"].Value<String>();
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

            // Audit retention.
            ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().AuditRetention = TimeSpan.Parse(optionObject["security"]["auditRetention"].Value<String>());

            if (optionObject["security"]["onlySubscribedAuth"].Value<Boolean>())
                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().OnlySubscribedFacilities = true;

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

            return new ConfigurationViewModel(XamarinApplicationContext.Current.Configuration);
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

            List<string> enableTrace = null, noTimeout = null, enableSSL = null, portNo;
            realmData.TryGetValue("enableTrace", out enableTrace);
            enableTrace = enableTrace ?? new List<String>();
            realmData.TryGetValue("noTimeout", out noTimeout);
            noTimeout = noTimeout ?? new List<String>();
            realmData.TryGetValue("enableSSL", out enableSSL);
            enableSSL = enableSSL ?? new List<string>();
            realmData.TryGetValue("port", out portNo);
            portNo = portNo ?? new List<string>() { "8080" };


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
                // Parse ACS URI
                var scheme = enableSSL.FirstOrDefault() == "true" ? "https" : "http";
                // AMI Client
                AmiServiceClient amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));

                // We get the ami options for the other configuration
                var serviceOptions = amiClient.Options();
                serviceClientSection.Client.Clear();

                Dictionary<ServiceEndpointType, String> endpointNames = new Dictionary<ServiceEndpointType, string>()
                {
                    { ServiceEndpointType.AdministrationIntegrationService, "ami" },
                    { ServiceEndpointType.ImmunizationIntegrationService, "imsi" },
                    { ServiceEndpointType.AuthenticationService, "acs" }
                };

                foreach (var itm in serviceOptions.Endpoints)
                {

                    var urlInfo = itm.BaseUrl.Where(o => o.StartsWith(scheme));
                    String serviceName = null;
                    if (!urlInfo.Any() || !endpointNames.TryGetValue(itm.ServiceType, out serviceName))
                        continue;

                    // Description binding
                    ServiceClientDescription description = new ServiceClientDescription()
                    {
                        Binding = new ServiceClientBinding()
                        {
                            Security = new ServiceClientSecurity()
                            {
                                AuthRealm = realmUri,
                                Mode = itm.Capabilities.HasFlag(ServiceEndpointCapabilities.BearerAuth) ? SecurityScheme.Bearer :
                                    itm.Capabilities.HasFlag(ServiceEndpointCapabilities.BasicAuth) ? SecurityScheme.Basic :
                                    SecurityScheme.None,
                                CredentialProvider = itm.Capabilities.HasFlag(ServiceEndpointCapabilities.BearerAuth) ? (ICredentialProvider)new TokenCredentialProvider() :
                                    itm.Capabilities.HasFlag(ServiceEndpointCapabilities.BasicAuth) ?
                                    (ICredentialProvider)(itm.ServiceType == ServiceEndpointType.AuthenticationService ? (ICredentialProvider)new OAuth2CredentialProvider() : new HttpBasicTokenCredentialProvider()) :
                                    null,
                                PreemptiveAuthentication = itm.Capabilities != ServiceEndpointCapabilities.None
                            },
                            Optimize = itm.Capabilities.HasFlag(ServiceEndpointCapabilities.Compression),
                        },
                        Endpoint = urlInfo.Select(o => new ServiceClientEndpoint()
                        {
                            Address = o.Replace("0.0.0.0", realmUri),
                            Timeout = itm.ServiceType == ServiceEndpointType.ImmunizationIntegrationService ? 60000 : 10000
                        }).ToList(),
                        Trace = enableTrace.Count > 0 && enableTrace[0] == "true",
                        Name = serviceName
                    };

                    serviceClientSection.Client.Add(description);
                }

                ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new AmiPolicyInformationService());
                ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new ImsiPersistenceService());

                byte[] pcharArray = Guid.NewGuid().ToByteArray();
                char[] spec = { '@', '#', '$', '*', '~' };
                for (int i = 0; i < pcharArray.Length; i++)
                    switch (i % 5)
                    {
                        case 0:
                            pcharArray[i] = (byte)((pcharArray[i] % 10) + 48);
                            break;
                        case 1:
                            pcharArray[i] = (byte)spec[pcharArray[i] % spec.Length];
                            break;
                        case 2:
                            pcharArray[i] = (byte)((pcharArray[i] % 25) + 65);
                            break;
                        case 3:
                            pcharArray[i] = (byte)((pcharArray[i] % 25) + 97);
                            break;
                        default:
                            pcharArray[i] = (byte)((pcharArray[i] % 61) + 65);
                            break;
                    }

                ApplicationContext.Current.Configuration.GetSection<SecurityConfigurationSection>().DeviceSecret = Encoding.ASCII.GetString(pcharArray);

                // Create the necessary device user
                try
                {
                    // Recreate the client with the updated security configuration
                    amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));
                    // Create application user
                    var role = amiClient.GetRoles(o => o.Name == "SYNCHRONIZERS").CollectionItem.First();


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
                            CreationTime = DateTimeOffset.Now,
                            UserName = deviceName,
                            Key = Guid.NewGuid(),
                            UserClass = UserClassKeys.ApplicationUser,
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
                        var newDevice = amiClient.CreateDevice(new SecurityDeviceInfo()
                        {
                            Device = new OpenIZ.Core.Model.Security.SecurityDevice()
                            {
                                CreationTime = DateTimeOffset.Now,
                                Name = deviceName,
                                DeviceSecret = Guid.NewGuid().ToString()
                            }
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
                catch (Exception e)
                {
                    this.m_tracer.TraceError("Error registering device account: {0}", e);
                    throw;
                }
                return new ConfigurationViewModel(XamarinApplicationContext.Current.Configuration);
            }
            catch (PolicyViolationException ex)
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

                // AMI Client
                serviceClientSection.Client.Clear();

                var scheme = enableSSL.FirstOrDefault() == "true" ? "https" : "http";
                string amiUri = String.Format("{0}://{1}:{2}/ami", scheme,
                    realmUri,
                    portNo.FirstOrDefault());
                serviceClientSection.Client.Add(new ServiceClientDescription()
                {
                    Binding = new ServiceClientBinding()
                    {
                        Optimize = false
                    },
                    Endpoint = new System.Collections.Generic.List<ServiceClientEndpoint>() {
                        new ServiceClientEndpoint() {
                            Address = amiUri, Timeout = 10000
                        }
                    },
                    Name = "ami",
                    Trace = enableTrace.Count > 0 && enableTrace[0] == "true"
                });


                AmiServiceClient amiClient = new AmiServiceClient(ApplicationContext.Current.GetRestClient("ami"));

                // We get the ami options for the other configuration
                var serviceOptions = amiClient.Options();

                var option = serviceOptions.Endpoints.FirstOrDefault(o => o.ServiceType == ServiceEndpointType.AuthenticationService);


                if (option == null)
                {
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new HttpBasicIdentityProvider());
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(HttpBasicIdentityProvider).AssemblyQualifiedName);
                }
                else
                {
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().Services.Add(new OAuthIdentityProvider());
                    ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection>().ServiceTypes.Add(typeof(OAuthIdentityProvider).AssemblyQualifiedName);
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
                        Name = "acs",
                        Trace = enableTrace.Count > 0 && enableTrace[0] == "true",
                        Endpoint = option.BaseUrl.Select(o => new ServiceClientEndpoint()
                        {
                            Address = o.Replace("0.0.0.0", realmUri),
                            Timeout = 10000
                        }).ToList()
                    });

                }

                throw new UnauthorizedAccessException();
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error joining context: {0}", e);
                throw;
            }
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
