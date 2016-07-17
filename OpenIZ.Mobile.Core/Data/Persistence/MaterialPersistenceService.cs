﻿using OpenIZ.Core.Model.Entities;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Entities;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Persistence service for matrials
    /// </summary>
    public class MaterialPersistenceService : EntityDerivedPersistenceService<Material, DbMaterial>
    {

        /// <summary>
        /// Convert persistence model to business objects
        /// </summary>
        public override Material ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            return this.ToModelInstance<Material>(dataInstance as DbMaterial, context);
        }

        /// <summary>
        /// Creates the specified model instance
        /// </summary>
        internal TModel ToModelInstance<TModel>(object rawInstance, SQLiteConnection context)
            where TModel : Material, new()
        {
            var iddat = rawInstance as DbVersionedData;
            var dataInstance = rawInstance as DbMaterial ?? context.Table<DbMaterial>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbe = rawInstance as DbEntity ?? context.Table<DbEntity>().Where(o => o.Uuid == dataInstance.Uuid).First();
            var retVal = this.m_entityPersister.ToModelInstance<TModel>(dbe, context);
            retVal.ExpiryDate = dataInstance.ExpiryDate;
            retVal.IsAdministrative = dataInstance.IsAdministrative;
            retVal.Quantity = dataInstance.Quantity;
            retVal.QuantityConceptKey = new Guid(dataInstance.QuantityConceptUuid);
            retVal.FormConceptKey = new Guid(dataInstance.FormConceptUuid);
            retVal.LoadAssociations(context);
            return retVal;

        }

        /// <summary>
        /// Insert the material
        /// </summary>
        public override Material Insert(SQLiteConnection context, Material data)
        {
            data.FormConcept?.EnsureExists(context);
            data.QuantityConcept?.EnsureExists(context);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified material
        /// </summary>
        public override Material Update(SQLiteConnection context, Material data)
        {
            data.FormConcept?.EnsureExists(context);
            data.QuantityConcept?.EnsureExists(context);
            data.FormConceptKey = data.FormConcept?.Key ?? data.FormConceptKey;
            data.QuantityConceptKey = data.QuantityConcept?.Key ?? data.QuantityConceptKey;
            return base.Update(context, data);
        }
    }
}
