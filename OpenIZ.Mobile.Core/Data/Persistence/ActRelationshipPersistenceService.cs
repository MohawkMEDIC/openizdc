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
    /// Represents a persister which is a act relationship 
    /// </summary>
    public class ActRelationshipPersistenceService : IdentifiedPersistenceService<ActRelationship, DbActRelationship>
    {

        /// <summary>
        /// Insert the relationship
        /// </summary>
        public override ActRelationship Insert(SQLiteConnectionWithLock context, ActRelationship data)
        {
            // Ensure we haven't already persisted this
            if(data.TargetAct != null) data.TargetAct = data.TargetAct.EnsureExists(context);
            data.TargetActKey = data.TargetAct?.Key ?? data.TargetActKey;
            if (data.RelationshipType != null) data.RelationshipType = data.RelationshipType.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;
            
            byte[] target = data.TargetActKey.Value.ToByteArray(),
                source = data.SourceEntityKey.Value.ToByteArray(),
                typeKey = data.RelationshipTypeKey.Value.ToByteArray();

            var existing = context.Table<DbActRelationship>().Where(o => o.TargetUuid == target && o.ActUuid == source && o.RelationshipTypeUuid == typeKey).FirstOrDefault();
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
        public override ActRelationship Update(SQLiteConnectionWithLock context, ActRelationship data)
        {
            if (data.TargetAct != null) data.TargetAct = data.TargetAct.EnsureExists(context);
            data.TargetActKey = data.TargetAct?.Key ?? data.TargetActKey;
            if (data.RelationshipType != null) data.RelationshipType = data.RelationshipType.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;

            return base.Update(context, data);
        }
    }
}
