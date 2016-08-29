using Newtonsoft.Json;
using OpenIZ.Core.Alert.Alerting;
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
    public class DbAlertMessage
    {

        public DbAlertMessage()
        {
            this.Id = Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// Creates a new alert message
        /// </summary>
        public DbAlertMessage(AlertMessage am)
        {
            this.TimeStamp = am.TimeStamp;
            this.From = am.From;
            this.Subject = am.Subject;
            this.Body = am.Body;
            this.To = am.To;
            this.CreatedBy = ApplicationContext.Current.Principal?.Identity.Name ?? "SYSTEM";
            this.Flags = am.Flags;
            this.Id = am.Id.ToByteArray();
        }

        /// <summary>
        /// Gets the alert
        /// </summary>
        public AlertMessage ToAlert()
        {
            return new AlertMessage(this.From, this.To, this.Subject, this.Body, this.Flags)
            {
                Id = new Guid(this.Id)
            };
        }

        /// <summary>
        /// Identifier
        /// </summary>
        [Column("key"), PrimaryKey, MaxLength(16)]
        public byte[] Id { get; set; }

        /// <summary>
        /// Gets or sets the time
        /// </summary>
        [Column("time")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the status of the alert
        /// </summary>
        [Column("flags"), Indexed]
        public AlertMessageFlags Flags { get; set; }

        /// <summary>
        /// The principal that created the message
        /// </summary>
        [Column("created_by")]
        public String CreatedBy { get; set; }

        /// <summary>
        /// Identifies the to
        /// </summary>
        [Column("to")]
        public String To { get; set; }

        /// <summary>
        /// Gets or sets the "from" subject if it is a human based message
        /// </summary>
        [Column("from")]
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the subject
        /// </summary>
        [Column("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// Body of the message
        /// </summary>
        [Column("body")]
        public string Body { get; set; }

    }

}