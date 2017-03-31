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
 * User: fyfej
 * Date: 2016-11-14
 */
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
            this.TimeStamp = am.TimeStamp.DateTime;
            this.From = am.From;
            this.Subject = am.Subject;
            this.Body = am.Body;
            this.To = am.To;
            this.CreatedBy = AuthenticationContext.Current.Principal?.Identity.Name ?? "SYSTEM";
            this.Flags = (int)am.Flags;
            this.Id = am.Key.HasValue ? am.Key.Value.ToByteArray() : Guid.NewGuid().ToByteArray();
        }

        /// <summary>
        /// Gets the alert
        /// </summary>
        public AlertMessage ToAlert()
        {
            return new AlertMessage(this.From, this.To, this.Subject, this.Body, (AlertMessageFlags)this.Flags)
            {
                Key = new Guid(this.Id),
				CreationTime = this.TimeStamp.GetValueOrDefault(),
                UpdatedTime = this.TimeStamp
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
        public DateTime? TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the status of the alert
        /// </summary>
        [Column("flags"), Indexed]
        public int Flags { get; set; }

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