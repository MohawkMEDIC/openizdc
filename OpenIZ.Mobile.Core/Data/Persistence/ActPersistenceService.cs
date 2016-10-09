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
 * Date: 2016-7-23
 */
using OpenIZ.Core.Model.Acts;
using OpenIZ.Mobile.Core.Data.Model.Acts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net;
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Mobile.Core.Data.Model.Extensibility;
using OpenIZ.Mobile.Core.Data.Model.DataType;

namespace OpenIZ.Mobile.Core.Data.Persistence
{
    /// <summary>
    /// Represents a persistence service which persists ACT classes
    /// </summary>
    public class ActPersistenceService : VersionedDataPersistenceService<Act, DbAct>
    {
        private const String ControlAct = "B35488CE-B7CD-4DD4-B4DE-5F83DC55AF9F";
        private const String SubstanceAdministration = "932A3C7E-AD77-450A-8A1F-030FC2855450";
        private const String Condition = "1987C53C-7AB8-4461-9EBC-0D428744A8C0";
        private const String Registration = "6BE8D358-F591-4A3A-9A57-1889B0147C7E";
        private const String Observation = "28D022C6-8A8B-47C4-9E6A-2BC67308739E";
        private const String Inform = "192F1768-D39E-409D-87BE-5AFD0EE0D1FE";
        private const String Encounter = "54B52119-1709-4098-8911-5DF6D6C84140";
        private const String Battery = "676DE278-64AA-44F2-9B69-60D61FC1F5F5";
        private const String Act = "D874424E-C692-4FD8-B94E-642E1CBF83E9";
        private const String Procedure = "8CC5EF0D-3911-4D99-937F-6CFDC2A27D55";
        private const String CareProvision = "1071D24E-6FE9-480F-8A20-B1825AE4D707";
        private const String AccountManagement = "CA44A469-81D7-4484-9189-CA1D55AFECBC";
        private const String Supply = "A064984F-9847-4480-8BEA-DDDF64B3C77C";

        /// <summary>
        /// To model instance
        /// </summary>
        public virtual TActType ToModelInstance<TActType>(DbAct dbInstance, SQLiteConnectionWithLock context, bool loadFast) where TActType : Act, new()
        {
            var retVal = m_mapper.MapDomainInstance<DbAct, TActType>(dbInstance);

            // Has this been updated? If so, minimal information about the previous version is available
            if(dbInstance.UpdatedTime != null)
            {
                retVal.CreationTime = (DateTimeOffset)dbInstance.UpdatedTime;
                retVal.CreatedByKey = dbInstance.UpdatedByKey;
                retVal.PreviousVersion = new Act()
                {
                    ClassConcept = retVal.ClassConcept,
                    MoodConcept = retVal.MoodConcept,
                    Key = dbInstance.Key,
                    VersionKey = dbInstance.PreviousVersionKey,
                    CreationTime = (DateTimeOffset)dbInstance.CreationTime,
                    CreatedByKey = dbInstance.CreatedByKey
                };
            }

            retVal.LoadAssociations(context);
            return retVal;
        }

        /// <summary>
        /// Create an appropriate entity based on the class code
        /// </summary>
        public override Act ToModelInstance(object dataInstance, SQLiteConnectionWithLock context, bool loadFast)
        {
            // Alright first, which type am I mapping to?
            var dbAct = dataInstance as DbAct;
            switch (new Guid(dbAct.ClassConceptUuid).ToString().ToUpper())
            {
                case ControlAct:
                    return new ControlActPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case SubstanceAdministration:
                    return new SubstanceAdministrationPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                case Condition:
                case Observation:
		            var dbObs = context.Table<DbObservation>().Where(o => o.Uuid == dbAct.Uuid).FirstOrDefault();

		            if (dbObs == null)
		            {
						return base.ToModelInstance(dataInstance, context, loadFast);
					}

                    switch(dbObs.ValueType)
                    {
                        case "ST":
                            return new TextObservationPersistenceService().ToModelInstance(
                                context.Table<DbTextObservation>().Where(o=>o.Uuid == dbObs.Uuid).First(),
                                dbAct, 
                                dbObs, 
                                context, 
                                loadFast);
                        case "CD":
                            return new CodedObservationPersistenceService().ToModelInstance(
                                context.Table<DbCodedObservation>().Where(o=>o.Uuid == dbObs.Uuid).First(),
                                dbAct, 
                                dbObs, 
                                context, 
                                loadFast);
                        case "PQ":
                            return new QuantityObservationPersistenceService().ToModelInstance(
                                context.Table<DbQuantityObservation>().Where(o=>o.Uuid == dbObs.Uuid).First(),
                                dbAct, 
                                dbObs, 
                                context,
                                loadFast);
                        default:
                            return base.ToModelInstance(dataInstance, context, loadFast);
                    }
                case Encounter:
                    return new EncounterPersistenceService().ToModelInstance(dataInstance, context, loadFast);
                default:
                    return this.ToModelInstance<Act>(dbAct, context, loadFast);

            }
        }

        /// <summary>
        /// Insert the act into the database
        /// </summary>
        internal Act InsertInternal(SQLiteConnectionWithLock context, Act data)
        {
            
            data.ClassConcept?.EnsureExists(context);
            data.MoodConcept?.EnsureExists(context);
            data.ReasonConcept?.EnsureExists(context);
            data.StatusConcept?.EnsureExists(context);
            data.TypeConcept?.EnsureExists(context);

            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.MoodConceptKey = data.MoodConcept?.Key ?? data.MoodConceptKey;
            data.ReasonConceptKey = data.ReasonConcept?.Key ?? data.ReasonConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey ?? StatusKeys.New;
            data.TypeConceptKey = data.TypeConcept?.Key ?? data.TypeConceptKey;

            // Do the insert
            var retVal = base.Insert(context, data);

            if (retVal.Extensions != null)
                base.UpdateAssociatedItems<ActExtension, Act>(
                    new List<ActExtension>(),
                    retVal.Extensions,
                    retVal.Key,
                    context);

            if (retVal.Identifiers != null)
                base.UpdateAssociatedItems<ActIdentifier, Act>(
                    new List<ActIdentifier>(),
                    retVal.Identifiers,
                    retVal.Key,
                    context);

            if (retVal.Notes != null)
                base.UpdateAssociatedItems<ActNote, Act>(
                    new List<ActNote>(),
                    retVal.Notes,
                    retVal.Key,
                    context);

            if (retVal.Participations != null)
                base.UpdateAssociatedItems<ActParticipation, Act>(
                    new List<ActParticipation>(),
                    retVal.Participations,
                    retVal.Key,
                    context);

            if (retVal.Relationships != null)
                base.UpdateAssociatedItems<ActRelationship, Act>(
                    new List<ActRelationship>(),
                    retVal.Relationships,
                    retVal.Key,
                    context);

            if (retVal.Tags != null)
                base.UpdateAssociatedItems<ActTag, Act>(
                    new List<ActTag>(),
                    retVal.Tags,
                    retVal.Key,
                    context);

            return retVal;
        }

        /// <summary>
        /// Update the specified data
        /// </summary>
        internal Act UpdateInternal(SQLiteConnectionWithLock context, Act data)
        {
            data.ClassConcept?.EnsureExists(context);
            data.MoodConcept?.EnsureExists(context);
            data.ReasonConcept?.EnsureExists(context);
            data.StatusConcept?.EnsureExists(context);
            data.TypeConcept?.EnsureExists(context);

            data.ClassConceptKey = data.ClassConcept?.Key ?? data.ClassConceptKey;
            data.MoodConceptKey = data.MoodConcept?.Key ?? data.MoodConceptKey;
            data.ReasonConceptKey = data.ReasonConcept?.Key ?? data.ReasonConceptKey;
            data.StatusConceptKey = data.StatusConcept?.Key ?? data.StatusConceptKey ?? StatusKeys.New;
            data.TypeConcept?.EnsureExists(context);

            // Do the update
            var retVal = base.Update(context, data);

            // Set appropriate versioning 
            retVal.PreviousVersion = new Act()
            {
                ClassConcept = retVal.ClassConcept,
                MoodConcept = retVal.MoodConcept,
                Key = retVal.Key,
                VersionKey = retVal.PreviousVersionKey,
                CreationTime = (DateTimeOffset)retVal.CreationTime,
                CreatedByKey = retVal.CreatedByKey
            };
            retVal.CreationTime = DateTimeOffset.Now;
            retVal.CreatedByKey = data.CreatedByKey == Guid.Empty || data.CreatedByKey == null ? base.CurrentUserUuid(context) : data.CreatedByKey;

            var ruuid = retVal.Key.Value.ToByteArray();

            if (retVal.Extensions != null)
                base.UpdateAssociatedItems<ActExtension, Act>(
                    context.Table<DbActExtension>().Where(a=>ruuid == a.ActUuid).ToList().Select(o=>m_mapper.MapDomainInstance<DbActExtension, ActExtension>(o)).ToList(),
                    retVal.Extensions,
                    retVal.Key,
                    context);

            if (retVal.Identifiers != null)
                base.UpdateAssociatedItems<ActIdentifier, Act>(
                    context.Table<DbActIdentifier>().Where(a => ruuid == a.ActUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbActIdentifier, ActIdentifier>(o)).ToList(),
                    retVal.Identifiers,
                    retVal.Key,
                    context);

            if (retVal.Notes != null)
                base.UpdateAssociatedItems<ActNote, Act>(
                    context.Table<DbActNote>().Where(a => ruuid == a.ActUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbActNote, ActNote>(o)).ToList(),
                    retVal.Notes,
                    retVal.Key,
                    context);

            if (retVal.Participations != null)
                base.UpdateAssociatedItems<ActParticipation, Act>(
                    context.Table<DbActParticipation>().Where(a => ruuid == a.ActUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbActParticipation, ActParticipation>(o)).ToList(),
                    retVal.Participations,
                    retVal.Key,
                    context);

            if (retVal.Relationships != null)
                base.UpdateAssociatedItems<ActRelationship, Act>(
                    context.Table<DbActRelationship>().Where(a => ruuid == a.ActUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbActRelationship, ActRelationship>(o)).ToList(),
                    retVal.Relationships,
                    retVal.Key,
                    context);

            if (retVal.Tags != null)
                base.UpdateAssociatedItems<ActTag, Act>(
                    context.Table<DbActTag>().Where(a => ruuid == a.ActUuid).ToList().Select(o => m_mapper.MapDomainInstance<DbActTag, ActTag>(o)).ToList(),
                    retVal.Tags,
                    retVal.Key,
                    context);

            return retVal;
        }

        /// <summary>
        /// Obsolete the act
        /// </summary>
        /// <param name="context"></param>
        internal Act ObsoleteInternal(SQLiteConnectionWithLock context, Act data)
        {
            data.StatusConceptKey = StatusKeys.Obsolete;
            return base.Obsolete(context, data);
        }

        /// <summary>
        /// Insert the specified act
        /// </summary>
        public override Act Insert(SQLiteConnectionWithLock context, Act data)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case ControlAct:
                    return new ControlActPersistenceService().Insert(context, data as ControlAct);
                case SubstanceAdministration:
                    return new SubstanceAdministrationPersistenceService().Insert(context, data as SubstanceAdministration);
                case Condition:
                case Observation:
                    switch (data.GetType().Name)
                    {
                        case "TextObservation":
                            return new TextObservationPersistenceService().Insert(context, data as TextObservation);
                        case "CodedObservation":
                            return new CodedObservationPersistenceService().Insert(context, data as CodedObservation);
                        case "QuantityObservation":
                            return new QuantityObservationPersistenceService().Insert(context, data as QuantityObservation);
                        default:
                            return this.InsertInternal(context, data);
                    }
                case Encounter:
                    return new EncounterPersistenceService().Insert(context, data as PatientEncounter);
                default:
                    return this.InsertInternal(context, data);

            }
            
        }

        /// <summary>
        /// Update the act
        /// </summary>
        public override Act Update(SQLiteConnectionWithLock context, Act data)
        {
            switch (data.ClassConceptKey.ToString().ToUpper())
            {
                case ControlAct:
                    return new ControlActPersistenceService().Update(context, data as ControlAct);
                case SubstanceAdministration:
                    return new SubstanceAdministrationPersistenceService().Update(context, data as SubstanceAdministration);
                case Condition:
                case Observation:
                    switch (data.GetType().Name)
                    {
                        case "TextObservation":
                            return new TextObservationPersistenceService().Update(context, data as TextObservation);
                        case "CodedObservation":
                            return new CodedObservationPersistenceService().Update(context, data as CodedObservation);
                        case "QuantityObservation":
                            return new QuantityObservationPersistenceService().Update(context, data as QuantityObservation);
                        default:
                            return this.UpdateInternal(context, data);
                    }
                case Encounter:
                    return new EncounterPersistenceService().Update(context, data as PatientEncounter);
                default:
                    return this.UpdateInternal(context, data);

            }
        }

        /// <summary>
        /// Obsolete
        /// </summary>
        public override Act Obsolete(SQLiteConnectionWithLock context, Act data)
        {
            return this.ObsoleteInternal(context, data);
        }

    }
}
