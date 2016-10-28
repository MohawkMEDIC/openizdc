using OpenIZ.Core.Model.Acts;
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

        /// <summary>
        /// Insert the relationship
        /// </summary>
        public override ActParticipation Insert(SQLiteConnectionWithLock context, ActParticipation data)
        {
            // Ensure we haven't already persisted this
            if(data.PlayerEntity != null) data.PlayerEntity = data.PlayerEntity.EnsureExists(context);
            data.PlayerEntityKey = data.PlayerEntity?.Key ?? data.PlayerEntityKey;
            if(data.ParticipationRole != null) data.ParticipationRole = data.ParticipationRole.EnsureExists(context);
            data.ParticipationRoleKey = data.ParticipationRole?.Key ?? data.ParticipationRoleKey;
            if (data.Act != null) data.Act = data.Act.EnsureExists(context);
            data.ActKey = data.Act?.Key ?? data.ActKey;

            byte[] target = data.PlayerEntityKey.Value.ToByteArray(),
                source = data.SourceEntityKey.Value.ToByteArray(),
                typeKey = data.ParticipationRoleKey.Value.ToByteArray();

            var existing = context.Table<DbActParticipation>().Where(o => o.EntityUuid == target && o.ActUuid == source && o.ParticipationRoleUuid == typeKey).FirstOrDefault();
            if (existing == null)
                return base.Insert(context, data);
            else
            {
                data.Key = new Guid(existing.Uuid);
                return data;
            }
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        public override ActParticipation Update(SQLiteConnectionWithLock context, ActParticipation data)
        {
            if (data.PlayerEntity != null) data.PlayerEntity = data.PlayerEntity.EnsureExists(context);
            data.PlayerEntityKey = data.PlayerEntity?.Key ?? data.PlayerEntityKey;
            if (data.ParticipationRole != null) data.ParticipationRole = data.ParticipationRole.EnsureExists(context);
            data.ParticipationRoleKey = data.ParticipationRole?.Key ?? data.ParticipationRoleKey;
            if (data.Act != null) data.Act = data.Act.EnsureExists(context);
            data.ActKey = data.Act?.Key ?? data.ActKey;

            return base.Update(context, data);
        }
    }
}
