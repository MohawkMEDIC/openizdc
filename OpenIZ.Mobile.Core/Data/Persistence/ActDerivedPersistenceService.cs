using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persistence service which is derived from an act
    /// </summary>
    public class ActDerivedPersistenceService<TModel, TData> : IdentifiedPersistenceService<TModel, TData>
        where TModel : Act, new()
        where TData : DbIdentified, new()
    {
        // act persister
        protected ActPersistenceService m_actPersister = new ActPersistenceService();

        /// <summary>
        /// Insert the specified TModel into the database
        /// </summary>
        public override TModel Insert(SQLiteConnection context, TModel data)
        {
            var inserted = this.m_actPersister.Insert(context, data);
            data.Key = inserted.Key;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified TModel
        /// </summary>
        public override TModel Update(SQLiteConnection context, TModel data)
        {
            this.m_actPersister.Update(context, data);
            return base.Update(context, data);
        }

        /// <summary>
        /// Obsolete the object
        /// </summary>
        public override TModel Obsolete(SQLiteConnection context, TModel data)
        {
            var retVal = this.m_actPersister.Obsolete(context, data);
            return data;
        }
    }
}
