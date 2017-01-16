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
 * User: fyfej
 * Date: 2016-10-25
 */
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Concept service
	/// </summary>
	public class LocalConceptService : IConceptRepositoryService
	{
		public IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<ConceptClass> FindConceptClasses(Expression<Func<ConceptClass, bool>> query, int offset, int? count, out int totalCount)
		{
			throw new NotImplementedException();
		}

        public IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ConceptReferenceTerm> FindConceptReferenceTerms(Expression<Func<ConceptReferenceTerm, bool>> query, int offset, int? count, out int totalCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find concepts
        /// </summary>
        public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query)
		{
			int total = 0;
			return this.FindConcepts(query, 0, null, out total);
		}

		/// <summary>
		/// Find concepts
		/// </summary>
		public IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			return persistenceService.Query(query, offset, count, out totalResults, Guid.Empty);
		}

		/// <summary>
		/// Locates the specified concepts by name
		/// </summary>
		public IEnumerable<Concept> FindConceptsByName(string name, string language)
		{
			return this.FindConcepts(o => o.ConceptNames.Any(n => n.Name == name && n.Language == language));
		}

		/// <summary>
		/// Find concepts by a reference term
		/// </summary>
		public IEnumerable<Concept> FindConceptsByReferenceTerm(string code, string codeSystemOid)
		{
			return this.FindConcepts(o => o.ReferenceTerms.Any(r => r.ReferenceTerm.CodeSystem.Oid == codeSystemOid && r.ReferenceTerm.Mnemonic == code));
		}

		/// <summary>
		/// Find concept sets that match the specified query
		/// </summary>
		public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query)
		{
			int total = 0;
			return this.FindConceptSets(query, 0, null, out total);
		}

		/// <summary>
		/// Find the specified concept sts
		/// </summary>
		public IEnumerable<ConceptSet> FindConceptSets(Expression<Func<ConceptSet, bool>> query, int offset, int? count, out int totalResults)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept set persistence service found");

			return persistenceService.Query(query, offset, count, out totalResults, Guid.Empty);
		}

        public IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ReferenceTerm> FindReferenceTerms(Expression<Func<ReferenceTerm, bool>> query, int offset, int? count, out int totalCount)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the specified concept by mnemonic
        /// </summary>
        public Concept GetConcept(string mnemonic)
		{
			return this.FindConcepts(o => o.Mnemonic == mnemonic).FirstOrDefault();
		}

		/// <summary>
		/// Get the specified concept
		/// </summary>
		public IdentifiedData GetConcept(Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept persistence service found");

			return persistenceService.Get(id);
		}

		public ConceptClass GetConceptClass(Guid id)
		{
			throw new NotImplementedException();
		}

        public ConceptReferenceTerm GetConceptReferenceTerm(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the specified concept set by identifier
        /// </summary>
        public ConceptSet GetConceptSet(Guid id)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No concept set persistence service found");

			return persistenceService.Get(id);
		}

		/// <summary>
		/// Get the specified concept set by mnemonic
		/// </summary>
		public ConceptSet GetConceptSet(string mnemonic)
		{
			return this.FindConceptSets(o => o.Mnemonic == mnemonic).FirstOrDefault();
		}

        public ReferenceTerm GetReferenceTerm(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the specified reference term for the concept
        /// </summary>
        public ReferenceTerm GetReferenceTerm(Concept concept, string codeSystemOid)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptReferenceTerm>>();
			if (persistenceService == null)
				throw new InvalidOperationException("No reference term persistence service found");
			return persistenceService.Query(o => o.SourceEntityKey == concept.Key && o.ReferenceTerm.CodeSystem.Oid == codeSystemOid).FirstOrDefault()?.ReferenceTerm;
		}

		public bool Implies(Concept a, Concept b)
		{
			throw new NotImplementedException();
		}

		public Concept InsertConcept(Concept concept)
		{
			throw new NotImplementedException();
		}

		public ConceptClass InsertConceptClass(ConceptClass conceptClass)
		{
			throw new NotImplementedException();
		}

        public ConceptReferenceTerm InsertConceptReferenceTerm(ConceptReferenceTerm conceptReferenceTerm)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create the specified concept st
        /// </summary>
        public ConceptSet InsertConceptSet(ConceptSet set)
		{
			throw new NotImplementedException();
		}

        public ReferenceTerm InsertReferenceTerm(ReferenceTerm referenceTerm)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determine if the concept set contains the specified concept
        /// </summary>
        public bool IsMember(ConceptSet set, Concept concept)
		{
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
			if (persistence == null)
				throw new InvalidOperationException("Cannot locate concept set persistence service");
			return persistence.Count(o => o.Concepts.Any(c => c.Key == concept.Key)) > 0;
		}

		public IdentifiedData ObsoleteConcept(Guid key)
		{
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();
            if (persistence == null)
                throw new InvalidOperationException("Cannot locate concept set persistence service");
            return persistence.Obsolete(new Concept() { Key = key });
        }

		public ConceptClass ObsoleteConceptClass(Guid key)
		{
			throw new NotImplementedException();
		}

        public ConceptReferenceTerm ObsoleteConceptReferenceTerm(Guid key)
        {
            throw new NotImplementedException();
        }

        public ConceptSet ObsoleteConceptSet(Guid key)
		{
            var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
            if (persistence == null)
                throw new InvalidOperationException("Cannot locate concept set persistence service");
            return persistence.Obsolete(new ConceptSet() { Key = key });

        }

        public Concept SaveConcept(Concept concept)
		{
			throw new NotImplementedException();
		}

		public Concept SaveConceptClass(ConceptClass clazz)
		{
			throw new NotImplementedException();
		}

        public ConceptReferenceTerm SaveConceptReferenceTerm(ConceptReferenceTerm conceptReferenceTerm)
        {
            throw new NotImplementedException();
        }

        public ConceptSet SaveConceptSet(ConceptSet set)
		{
			throw new NotImplementedException();
		}

		public Concept SaveReferenceTerm(ReferenceTerm term)
		{
			throw new NotImplementedException();
		}

        Concept IConceptRepositoryService.ObsoleteConcept(Guid key)
        {
            throw new NotImplementedException();
        }

        ConceptClass IConceptRepositoryService.SaveConceptClass(ConceptClass conceptClass)
        {
            throw new NotImplementedException();
        }

        ReferenceTerm IConceptRepositoryService.SaveReferenceTerm(ReferenceTerm term)
        {
            throw new NotImplementedException();
        }
    }
}