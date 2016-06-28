using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Entity derived persistence services
    /// </summary>
    public class EntityDerivedPersistenceService<TModel, TData> : IdentifiedPersistenceService<TModel, TData>
        where TModel : Entity, new()
        where TData : DbIdentified, new()
    {

        // Entity persister
        protected EntityPersistenceService m_entityPersister = new EntityPersistenceService();


        /// <summary>
        /// Insert the specified TModel into the database
        /// </summary>
        public override TModel Insert(SQLiteConnection context, TModel data)
        {
            var inserted = this.m_entityPersister.Insert(context, data);
            data.Key = inserted.Key;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        public override TModel Update(SQLiteConnection context, TModel data)
        {
            this.m_entityPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override TModel Obsolete(SQLiteConnection context, TModel data)
        {
            var retVal = this.m_entityPersister.Obsolete(context, data);
            return data;
        }

    }
}