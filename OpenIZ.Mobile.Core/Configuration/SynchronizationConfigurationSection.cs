using Newtonsoft.Json;
using OpenIZ.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Configuration
{
    /// <summary>
    /// Configuration related to synchronization
    /// </summary>
	[XmlType(nameof(SynchronizationConfigurationSection), Namespace = "http://openiz.org/mobile/configuration")]
    public class SynchronizationConfigurationSection : IConfigurationSection
    {
        /// <summary>
        /// Synchronization configuration section ctor
        /// </summary>
        public SynchronizationConfigurationSection()
        {
            this.SynchronizationResources = new List<SynchronizationResource>();
        }

        /// <summary>
        /// Gets or sets the list of synchronization queries
        /// </summary>
        [XmlElement("sync")]
        public List<SynchronizationResource> SynchronizationResources { get; set; }
    }

    /// <summary>
    /// Synchronization 
    /// </summary>
    [XmlType(nameof(SynchronizationResource), Namespace = "http://openiz.org/mobile/configuration")]
    public class SynchronizationResource
    {
        /// <summary>
        /// default ctor
        /// </summary>
        public SynchronizationResource()
        {
            this.Filters = new List<string>();
        }

        /// <summary>
        /// Gets the resource type
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ResourceType
        {
            get; set;
        }

        /// <summary>
        /// Represents the triggers
        /// </summary>
        [XmlAttribute("trigger")]
        public SynchronizationPullTriggerType Triggers { get; set; }

        /// <summary>
        /// Gets or sets the resource type
        /// </summary>
        [XmlAttribute("resourceType")]
        public string ResourceAqn
        {
            get
            {
                return this.ResourceType.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>().TypeName;
            }
            set
            {
                this.ResourceType = typeof(IdentifiedData).GetTypeInfo().Assembly.ExportedTypes.First(o => o.GetTypeInfo().GetCustomAttribute<XmlTypeAttribute>()?.TypeName == value);
            }
        }

        /// <summary>
        /// One or more filters 
        /// </summary>
        [XmlElement("filter")]
        public List<string> Filters { get; set; }
    }


    /// <summary>
    /// Represents synchronization pull triggers
    /// </summary>
    [XmlType(nameof(SynchronizationPullTriggerType), Namespace = "http://openiz.org/mobile/configuration")]
    public enum SynchronizationPullTriggerType
    {
        Never = 0x0,
        Always = OnStart | OnCommit | OnStop | OnPush,
        OnStart = 0x01,
        OnCommit = 0x02,
        OnStop = 0x04,
        OnPush = 0x08
    }
}