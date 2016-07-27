using Newtonsoft.Json;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Alerting
{

    /// <summary>
    /// A message
    /// </summary>
    [JsonObject, Table("alert"), XmlType(nameof(AlertMessage), Namespace = "http://openiz.org/alerting")]
    public class AlertMessage
    {

        public AlertMessage()
        {

        }

        /// <summary>
        /// Creates a new alert message
        /// </summary>
        public AlertMessage(String from, String to, String subject, String body, AlertMessageFlags flags = AlertMessageFlags.None)
        {
            this.From = from;
            this.Subject = subject;
            this.Body = body;
            this.To = to;
            this.CreatedBy = ApplicationContext.Current.Principal.Identity.Name;
            this.Flags = flags;
        }

        /// <summary>
        /// Gets or sets the id of the alert
        /// </summary>
        [JsonProperty("id"), XmlElement("id"), Ignore]
        public Guid Id { get { return new Guid(this.Key); } set { this.Key = value.ToByteArray(); } }

        
        /// <summary>
        /// The key for data storage
        /// </summary>
        [JsonIgnore, XmlIgnore, Column("key"), PrimaryKey, MaxLength(16)]
        public byte[] Key { get; set; }

        /// <summary>
        /// Gets or sets the time
        /// </summary>
        [JsonProperty("time"), XmlElement("time"), Column("time")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the status of the alert
        /// </summary>
        [JsonProperty("flags"), XmlElement("flags"), Column("flags"), Indexed]
        public AlertMessageFlags Flags { get; set; }

        /// <summary>
        /// The principal that created the message
        /// </summary>
        [JsonProperty("createdBy"), XmlElement("createdBy"), Column("created_by")]
        public String CreatedBy { get; set; }

        /// <summary>
        /// Identifies the to
        /// </summary>
        [JsonProperty("to"), XmlElement("to"), Column("to")]
        public String To { get; set; }

        /// <summary>
        /// Gets or sets the "from" subject if it is a human based message
        /// </summary>
        [JsonProperty("from"), XmlElement("from"), Column("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [JsonProperty("subject"), XmlElement("subject"), Column("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Body of the message
        /// </summary>
        [JsonProperty("body"), XmlElement("body"), Column("body")]
        public string Body { get; set; }

    }

    /// <summary>
    /// Message status type
    /// </summary>
    public enum AlertMessageFlags
    {
        /// <summary>
        /// Just a normal alert
        /// </summary>
        None = 0x0,
        /// <summary>
        /// Indicates the message requires some immediate action!
        /// </summary>
        Alert = 0x1,
        /// <summary>
        /// Indicates whether someone has acknowledged the alert
        /// </summary>
        Acknowledged = 0x2,
        /// <summary>
        /// Indicates the alert is high priority but doesn't require immediate action
        /// </summary>
        HighPriority = 0x4,
        /// <summary>
        /// Indicates the alert is a system alert
        /// </summary>
        System = 0x8,
        /// <summary>
        /// Indicates the alert is transient and shouldn't be persisted
        /// </summary>
        Transient = 0x10,
        HighPriorityAlert = HighPriority | Alert
    }
}