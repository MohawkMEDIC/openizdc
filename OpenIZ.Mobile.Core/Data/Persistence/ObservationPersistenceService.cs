/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-24
 */
using System;
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using SQLite.Net;

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
        public virtual TObservation ToModelInstance(TDbObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var retVal = m_actPersister.ToModelInstance<TObservation>(actInstance, context, loadFast);

            if(obsInstance.InterpretationConceptUuid != null)
                retVal.InterpretationConceptKey = new Guid(obsInstance.InterpretationConceptUuid);

            return retVal;
        }

        /// <summary>
        /// Insert the specified observation into the database
        /// </summary>
        public override TObservation Insert(SQLiteConnectionWithLock context, TObservation data)
        {
            data.InterpretationConcept?.EnsureExists(context);
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;
            
            var inserted = base.Insert(context, data);

            // Not pure observation
            if (data.GetType() != typeof(Observation))
            {
                var dbobservation = new DbObservation()
                {
                    InterpretationConceptUuid = data.InterpretationConceptKey?.ToByteArray(),
                    Uuid = inserted.Key?.ToByteArray()
                };
                // Value type
                if (data is QuantityObservation)
                    dbobservation.ValueType = "PQ";
                else if (data is TextObservation)
                    dbobservation.ValueType = "ST";
                else if (data is CodedObservation)
                    dbobservation.ValueType = "CD";

                // Persist
                context.Insert(dbobservation);
            }
            return inserted;
        }

        /// <summary>
        /// Updates the specified observation
        /// </summary>
        public override TObservation Update(SQLiteConnectionWithLock context, TObservation data)
        {
            data.InterpretationConcept?.EnsureExists(context);
            data.InterpretationConceptKey = data.InterpretationConcept?.Key ?? data.InterpretationConceptKey;

            var updated = base.Update(context, data);

            // Not pure observation
            if (data.GetType() != typeof(Observation))
            {
                var dbobservation = new DbObservation()
                {
                    InterpretationConceptUuid = data.InterpretationConceptKey?.ToByteArray(),
                    Uuid = updated.Key?.ToByteArray()
                };
                context.Update(dbobservation);
            }
            return updated;
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
        public override TextObservation ToModelInstance(DbTextObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context, loadFast);
            retVal.Value = dataInstance.Value;
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override TextObservation ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var iddat = dataInstance as DbVersionedData;
            var textObs = dataInstance as DbTextObservation ?? context.Table<DbTextObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbo = context.Table<DbObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            return this.ToModelInstance(textObs, dba, dbo, context, loadFast);
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
        public override CodedObservation ToModelInstance(DbCodedObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context, loadFast);
            if(dataInstance.Value != null)
                retVal.ValueKey = new Guid(dataInstance.Value);
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override CodedObservation ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var iddat = dataInstance as DbVersionedData;
            var codeObs = dataInstance as DbCodedObservation ?? context.Table<DbCodedObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(o => o.Uuid == iddat.Uuid).First();
            var dbo = context.Table<DbObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            return this.ToModelInstance(codeObs, dba, dbo, context, loadFast);
        }

        /// <summary>
        /// Insert the observation
        /// </summary>
        public override CodedObservation Insert(SQLiteConnectionWithLock context, CodedObservation data)
        {
            data.Value?.EnsureExists(context);
            data.ValueKey = data.Value?.Key ?? data.ValueKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override CodedObservation Update(SQLiteConnectionWithLock context, CodedObservation data)
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
        public override QuantityObservation ToModelInstance(DbQuantityObservation dataInstance, DbAct actInstance, DbObservation obsInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var retVal = base.ToModelInstance(dataInstance, actInstance, obsInstance, context, loadFast);
            if (dataInstance.UnitOfMeasureUuid != null)
                retVal.UnitOfMeasureKey = new Guid(dataInstance.UnitOfMeasureUuid);
            retVal.Value = dataInstance.Value;
            return retVal;
        }

        /// <summary>
        /// Convert to model instance
        /// </summary>
        public override QuantityObservation ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            var iddat = dataInstance as DbVersionedData;
            var qObs = dataInstance as DbQuantityObservation ?? context.Table<DbQuantityObservation>().Where(o => o.Uuid == iddat.Uuid).First();
            var dba = dataInstance as DbAct ?? context.Table<DbAct>().Where(o => o.Uuid == qObs.Uuid).First();
            var dbo = context.Table<DbObservation>().Where(o => o.Uuid == qObs.Uuid).First();
            return this.ToModelInstance(qObs, dba, dbo, context, loadFast);
        }

        /// <summary>
        /// Insert the observation
        /// </summary>
        public override QuantityObservation Insert(SQLiteConnectionWithLock context, QuantityObservation data)
        {
            data.UnitOfMeasure?.EnsureExists(context);
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            return base.Insert(context, data);
        }

        /// <summary>
        /// Update the specified observation
        /// </summary>
        public override QuantityObservation Update(SQLiteConnectionWithLock context, QuantityObservation data)
        {
            data.UnitOfMeasure?.EnsureExists(context);
            data.UnitOfMeasureKey = data.UnitOfMeasure?.Key ?? data.UnitOfMeasureKey;
            return base.Update(context, data);
        }
    }
}