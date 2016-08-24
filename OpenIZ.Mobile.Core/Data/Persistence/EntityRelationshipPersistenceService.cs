using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity relationship persistence service
    /// </summary>
    public class EntityRelationshipPersistenceService : IdentifiedPersistenceService<EntityRelationship, DbEntityRelationship>
    {

        /// <summary>
        /// Insert the relationship
        /// </summary>
        public override EntityRelationship Insert(SQLiteConnectionWithLock context, EntityRelationship data)
        {
            data.TargetEntity?.EnsureExists(context);
            data.TargetEntityKey = data.TargetEntity?.Key ?? data.TargetEntityKey;
            data.RelationshipType?.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified object
        /// </summary>
        public override EntityRelationship Update(SQLiteConnectionWithLock context, EntityRelationship data)
        {
            data.TargetEntity?.EnsureExists(context);
            data.TargetEntityKey = data.TargetEntity?.Key ?? data.TargetEntityKey;
            data.RelationshipType?.EnsureExists(context);
            data.RelationshipTypeKey = data.RelationshipType?.Key ?? data.RelationshipTypeKey;
            return base.Update(context, data);
        }
    }
}
