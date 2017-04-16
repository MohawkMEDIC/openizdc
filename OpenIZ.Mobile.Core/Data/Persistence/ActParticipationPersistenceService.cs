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
using OpenIZ.Core.Data.QueryBuilder;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using SQLite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Act participation persistence service
    /// </summary>
    public class ActParticipationPersistenceService : IdentifiedPersistenceService<ActParticipation, DbActParticipation>
    {
        // Role mnemonics to save from hitting the DB
        private readonly Dictionary<String, String> m_roleMnemonicDictionary = new Dictionary<String, String>() {
            { "A0174216-6439-4351-9483-A241A48029B7", "Admitter" },
            { "6CBF29AD-AC51-48C9-885A-CFE3026ECF6E", "Attender" },
            { "1B2DBF82-A503-4CF4-9ECB-A8E111B4674E", "Authenticator" },
            { "F0CB3FAF-435D-4704-9217-B884F757BC14", "Authororiginator" },
            { "479896B0-35D5-4842-8109-5FDBEE14E8A4", "Baby" },
            { "28C744DF-D889-4A44-BC1A-2E9E9D64AF13", "Beneficiary" },
            { "9C4C40AE-2C15-4581-A496-BE1ABFE4EB66", "CallbackContact" },
            { "7F81B83E-0D78-4685-8BA4-224EB315CE54", "CausativeAgent" },
            { "0A364AD7-F961-4D8A-93F0-1FD4176548B3", "Consultant" },
            { "A5CAC7F7-E3B7-4DD8-872C-DB0E7FCC2D84", "Consumable" },
            { "4B5471D4-E3FE-45F7-85A2-AE2B4F224757", "CoverageTarget" },
            { "649D6D69-139C-4006-AE45-AFF4649D6079", "Custodian" },
            { "C50D66D2-E5DA-4A34-B2B7-4CD4FE4EF2C4", "DataEnterer" },
            { "727B3624-EA62-46BB-A68B-B9E49E302ECA", "Destination" },
            { "1373FF04-A6EF-420A-B1D0-4A07465FE8E8", "Device" },
            { "D9F63423-BA9B-48D9-BA38-C404B784B670", "DirectTarget" },
            { "A2594E6E-E8FE-4C68-82A5-D3A46DBEC87D", "Discharger" },
            { "693F08FA-625A-40D2-B928-6856099C0349", "Distributor" },
            { "BE1235EE-710A-4732-88FD-6E895DE7C56D", "Donor" },
            { "AC05185B-5A80-47A8-B924-060DEB6D0EB2", "EntryLocation" },
            { "727A61ED-2F35-4E09-8BB6-6D09E2BA8FEC", "Escort" },
            { "5A6A6766-8E1D-4D36-AE50-9B7D82D8A182", "Exposure" },
            { "EA60A5A9-E971-4F0D-BB5D-DC7A0C74A2C9", "ExposureAgent" },
            { "CBB6297B-743C-453C-8476-BA4C10A1C965", "ExposureSource" },
            { "EC401B5C-4C33-4229-9C72-428FC5DB37FF", "ExposureTarget" },
            { "28FB791E-179E-461A-B16C-CAC13A04BD0A", "GuarantorParty" },
            { "2452B691-F122-4121-B9DF-76D990B43F35", "Holder" },
            { "3A9F0C2F-E322-4639-A8E7-0DF67CAC761B", "IndirectTarget" },
            { "39604248-7812-4B60-BC54-8CC1FFFB1DE6", "Informant" },
            { "9790B291-B8A3-4C85-A240-C2C38885AD5D", "InformationRecipient" },
            { "0716A333-CD46-439D-BFD6-BF788F3885FA", "LegalAuthenticator" },
            { "61848557-D78D-40E5-954F-0B9C97307A04", "Location" },
            { "6792DB6C-FD5C-4AB8-96F5-ACE5665BDCB9", "NonreuseableDevice" },
            { "5D175F21-1963-4589-A400-B5EF5F64842C", "Origin" },
            { "C704A23D-86EF-4E11-9050-F8AA10919FF2", "Participation" },
            { "FA5E70A4-A46E-4665-8A20-94D4D7B86FC8", "Performer" },
            { "02BB7934-76B5-4CC5-BD42-58570F15EB4D", "PrimaryInformationRecipient" },
            { "79F6136C-1465-45E8-917E-E7832BC8E3B2", "PrimaryPerformer" },
            { "99E77288-CB09-4050-A8CF-385513F32F0A", "Product" },
            { "53C694B8-27D8-43DD-95A4-BB318431D17C", "Receiver" },
            { "3F92DBEE-A65E-434F-98CE-841FEEB02E3F", "RecordTarget" },
            { "6DA3A6CA-2AB0-4D32-9588-E094F277F06D", "ReferredBy" },
            { "353F9255-765E-4336-8007-1D61AB09AAD6", "ReferredTo" },
            { "5E8E0F8B-BC23-4847-82AB-49B8DD79981E", "Referrer" },
            { "3C1225DE-194E-49CE-A41A-0F9376B04C11", "Remote" },
            { "64474C12-B978-4BB6-A584-46DADEC2D952", "ResponsibleParty" },
            { "76990D3D-3F27-4B39-836B-BA87EEBA3328", "ReusableDevice" },
            { "4FF91E06-2E39-44E3-9FBE-0D828FE318FE", "SecondaryPerformer" },
            { "BCE17B21-05B2-4F02-BF7A-C6D3561AA948", "Specimen" },
            { "03067700-CE37-405F-8ED3-E4965BA2F601", "Subject" },
            { "C3BE013A-20C5-4C20-840C-D9DBB15D040E", "Tracker" },
            { "DE3F7527-E3C9-45EF-8574-00CA4495F767", "Transcriber" },
            { "01B87999-85A7-4F5C-9B7E-892F1195CFE3", "UgentNotificationContact" },
            { "F9DC5787-DD4D-42C6-A082-AC7D11956FDA", "Verifier" },
            { "5B0FAC74-5AC6-44E6-99A4-6813C0E2F4A9", "Via" },
            { "0B82357F-5AE0-4543-AB8E-A33E9B315BAB", "Witness" }
        };

        /// <summary>
        /// To model instance 
        /// </summary>
        public override ActParticipation ToModelInstance(object dataInstance, LocalDataContext context)
        {
            var dbi = dataInstance as DbActParticipation;
            if (dbi == null) return null;

            var roleKey = new Guid(dbi.ParticipationRoleUuid);
            return new ActParticipation()
            {
                ActKey = new Guid(dbi.ActUuid),
                LoadState = OpenIZ.Core.Model.LoadState.FullLoad,
                ParticipationRoleKey = roleKey,
                ParticipationRole = new Concept()
                {
                    Key = roleKey,
                    Mnemonic = this.m_roleMnemonicDictionary[roleKey.ToString().ToUpper()]
                },
                PlayerEntityKey = new Guid(dbi.EntityUuid),
                Quantity = dbi.Quantity,
                Key = new Guid(dbi.Uuid)
            };
        }

        /// <summary>
        /// Create DbActParticipation from modelinstance
        /// </summary>
        public override object FromModelInstance(ActParticipation modelInstance, LocalDataContext context)
        {
            modelInstance.Key = modelInstance.Key ?? Guid.NewGuid();
            return new DbActParticipation()
            {
                ActUuid = modelInstance.ActKey?.ToByteArray(),
                EntityUuid = modelInstance.PlayerEntityKey?.ToByteArray(),
                ParticipationRoleUuid = modelInstance.ParticipationRoleKey?.ToByteArray(),
                Uuid = modelInstance.Key?.ToByteArray() 
            };
        }

        /// <summary>
        /// Insert the relationship
        /// </summary>
        protected override ActParticipation InsertInternal(LocalDataContext context, ActParticipation data)
        {
            // Ensure we haven't already persisted this
            if (data.PlayerEntity != null) data.PlayerEntity = data.PlayerEntity.EnsureExists(context);
            data.PlayerEntityKey = data.PlayerEntity?.Key ?? data.PlayerEntityKey;
            if (data.ParticipationRole != null) data.ParticipationRole = data.ParticipationRole.EnsureExists(context);
            data.ParticipationRoleKey = data.ParticipationRole?.Key ?? data.ParticipationRoleKey;
            if (data.Act != null) data.Act = data.Act.EnsureExists(context);
            data.ActKey = data.Act?.Key ?? data.ActKey;

            byte[] target = data.PlayerEntityKey.Value.ToByteArray(),
                source = data.SourceEntityKey.Value.ToByteArray(),
                typeKey = data.ParticipationRoleKey.Value.ToByteArray();

            SqlStatement sql = new SqlStatement<DbActParticipation>().SelectFrom(o => o.Uuid)
               .Where<DbActParticipation>(o => o.ActUuid == source && o.EntityUuid == target && o.ParticipationRoleUuid == typeKey)
               .Limit(1).Build();

            var existing = context.Connection.Query<DbIdentified>(sql.SQL, sql.Arguments.ToArray()).FirstOrDefault();
            if (existing == null)
                return base.InsertInternal(context, data);
            else
            {
                data.Key = new Guid(existing.Uuid);
                return data;
            }
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        protected override ActParticipation UpdateInternal(LocalDataContext context, ActParticipation data)
        {
            if (data.PlayerEntity != null) data.PlayerEntity = data.PlayerEntity.EnsureExists(context);
            data.PlayerEntityKey = data.PlayerEntity?.Key ?? data.PlayerEntityKey;
            if (data.ParticipationRole != null) data.ParticipationRole = data.ParticipationRole.EnsureExists(context);
            data.ParticipationRoleKey = data.ParticipationRole?.Key ?? data.ParticipationRoleKey;
            if (data.Act != null) data.Act = data.Act.EnsureExists(context);
            data.ActKey = data.Act?.Key ?? data.ActKey;

            return base.UpdateInternal(context, data);
        }
    }
}
