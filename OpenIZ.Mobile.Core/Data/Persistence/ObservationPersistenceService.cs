using System;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using SQLite;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Persistence class for observations
    /// </summary>
    public abstract class ObservationPersistenceService<TObservation, TDbObservation> : ActDerivedPersistenceService<TObservation, TDbObservation>
        where TObservation : Observation, new()
        where TDbObservation : DbIdentified, new()
    {

        /// <summary>
        /// Convert a data act and observation instance to an observation
        /// </summary>
        public virtual TObservation ToModelInstance(TDbObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnection context)
        {
            var retVal = m_actPersister.ToModelInstance<TObservation>(actInstance, context);

            if(obsInstance.InterpretationConceptUuid != null)
                retVal.InterpretationConceptKey = new Guid(obsInstance.InterpretationConceptUuid);

            return retVal;
        }

        /// <summary>
        /// Insert the specified observation into the database
        /// </summary>
        public override TObservation Insert(SQLiteConnection context, TObservation data)
        {
            data.InterpretationConcept?.EnsureExists(context);
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;

            return base.Insert(context, data);
        }

        /// <summary>
        /// Updates the specified observation
        /// </summary>
        public override TObservation Update(SQLiteConnection context, TObservation data)
        {
            data.InterpretationConcept?.EnsureExists(context);
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;

            return base.Update(context, data);
        }
    }

    /// <summary>
    /// Text observation service
    /// </summary>
    public class TextObservationPersistenceService : ObservationPersistenceService<TextObservation, DbTextObservation>
    {
        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public override TextObservation ToModelInstance(DbTextObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnection context)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context);
            retVal.Value = dataInstance.Value;
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override TextObservation ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var iddat = dataInstance as DbVersionedData;
            var textObs = dataInstance as DbTextObservation ?? context.Table<DbTextObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbo = context.Table<DbObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            return this.ToModelInstance(textObs, dba, dbo, context);
        }
    }

    /// <summary>
    /// Coded observation service
    /// </summary>
    public class CodedObservationPersistenceService : ObservationPersistenceService<CodedObservation, DbCodedObservation>
    {
        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public override CodedObservation ToModelInstance(DbCodedObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnection context)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context);
            if(dataInstance.Value != null)
                retVal.ValueKey = new Guid(dataInstance.Value);
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override CodedObservation ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var iddat = dataInstance as DbVersionedData;
            var codeObs = dataInstance as DbCodedObservation ?? context.Table<DbCodedObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbo = context.Table<DbObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            return this.ToModelInstance(codeObs, dba, dbo, context);
        }

        /// <summary>
        /// Insert the observation
        /// </summary>
        public override CodedObservation Insert(SQLiteConnection context, CodedObservation data)
        {
            data.Value?.EnsureExists(context);
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override CodedObservation Update(SQLiteConnection context, CodedObservation data)
        {
            data.Value?.EnsureExists(context);
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            return base.Update(context, data);
        }
    }

    /// <summary>
    /// Quantity observation persistence service
    /// </summary>
    public class QuantityObservationPersistenceService : ObservationPersistenceService<QuantityObservation, DbQuantityObservation>
    {
        /// <summary>
        /// Convert the specified object to a model instance
        /// </summary>
        public override QuantityObservation ToModelInstance(DbQuantityObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnection context)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context);
            if (dataInstance.UnitOfMeasureUuid != null)
                retVal.UnitOfMeasureKey = new Guid(dataInstance.UnitOfMeasureUuid);
            retVal.Value = dataInstance.Value;
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override QuantityObservation ToModelInstance(object dataInstance, SQLiteConnection context)
        {
            var iddat = dataInstance as DbVersionedData;
            var qObs = dataInstance as DbQuantityObservation ?? context.Table<DbQuantityObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbo = context.Table<DbObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            return this.ToModelInstance(qObs, dba, dbo, context);
        }

        /// <summary>
        /// Insert the observation
        /// </summary>
        public override QuantityObservation Insert(SQLiteConnection context, QuantityObservation data)
        {
            data.UnitOfMeasure?.EnsureExists(context);
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override QuantityObservation Update(SQLiteConnection context, QuantityObservation data)
        {
            data.UnitOfMeasure?.EnsureExists(context);
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            return base.Update(context, data);
        }
    }
}