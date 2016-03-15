using System;
using System.Linq;
using OpenIZ.Core.Model.DataTypes;
using System.Collections.Generic;
using OpenIZ.Core.Model.Constants;

namespace OpenIZ.Mobile.Core.Services.Impl
{
	/// <summary>
	/// Local concept service.
	/// </summary>
	public class LocalConceptService : IConceptService
	{
		#region IConceptService implementation
		/// <summary>
		/// Get the specified concept set
		/// </summary>
		/// <returns>The concept set.</returns>
		/// <param name="mnemonic">Mnemonic.</param>
		public OpenIZ.Core.Model.DataTypes.ConceptSet GetConceptSet (string mnemonic)
		{
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>> ();
			return persistence.Query (o => o.Mnemonic == mnemonic).FirstOrDefault ();
		}

		/// <summary>
		/// Get a concept by its mnemonic
		/// </summary>
		/// <param name="mnemonic">The concept mnemonic to get</param>
		/// <returns>The concept.</returns>
		public OpenIZ.Core.Model.DataTypes.Concept GetConcept (string mnemonic)
		{
			var persistence = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>> ();
			return persistence.Query (o => o.Mnemonic == mnemonic).FirstOrDefault();
		}

		/// <summary>
		/// Performs an arbirary query
		/// </summary>
		/// <param name="query">The query to execute</param>
		/// <returns>The concepts.</returns>
		public System.Collections.Generic.IEnumerable<OpenIZ.Core.Model.DataTypes.Concept> FindConcepts (System.Linq.Expressions.Expression<Func<OpenIZ.Core.Model.DataTypes.Concept, bool>> query)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>> ();
			var results = persistenceService.Query (query);
			return results;
		}
		/// <summary>
		/// Finds a series of concepts by name
		/// </summary>
		/// <returns>The concepts by name.</returns>
		/// <param name="name">Name.</param>
		/// <param name="language">Language.</param>
		public System.Collections.Generic.IEnumerable<OpenIZ.Core.Model.DataTypes.Concept> FindConceptsByName (string name, string language)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>> ();
			return persistenceService.Query (nameof(FindConceptsByName), new Dictionary<String, Object>() {
				{ "Name", name },
				{ "Language", language }
			});
		}
		/// <summary>
		/// Find a reference term by code system oid
		/// </summary>
		/// <param name="code">The code</param>
		/// <param name="codeSystemOid">The oid of the code system</param>
		/// <returns>The concepts by reference term.</returns>
		public System.Collections.Generic.IEnumerable<OpenIZ.Core.Model.DataTypes.Concept> FindConceptsByReferenceTerm (string code, string codeSystemOid)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>> ();
			return persistenceService.Query (nameof (FindConceptsByReferenceTerm), new Dictionary<String, Object> () {
				{ "Mnemonic", code },
				{ "CodeSystem.Oid", codeSystemOid }
			});
		}
		/// <summary>
		/// Implies the specified a and b.
		/// </summary>
		/// <param name="a">The alpha component.</param>
		/// <param name="b">The blue component.</param>
		public bool Implies (OpenIZ.Core.Model.DataTypes.Concept a, OpenIZ.Core.Model.DataTypes.Concept b)
		{
			return a.Relationship.Exists(r => r.TargetConcept.Key == b.Key && r.RelationshipTypeKey == ConceptRelationshipTypeKeys.SameAs);
		}
		/// <summary>
		/// Determines whether this instance is member the specified set concept.
		/// </summary>
		/// <returns><c>true</c> if this instance is member the specified set concept; otherwise, <c>false</c>.</returns>
		/// <param name="set">Set.</param>
		/// <param name="concept">Concept.</param>
		public bool IsMember (OpenIZ.Core.Model.DataTypes.ConceptSet set, OpenIZ.Core.Model.DataTypes.Concept concept)
		{
			return set.Concepts.Exists (o => o.Key == concept.Key);
		}
		/// <summary>
		/// Gets the reference term.
		/// </summary>
		/// <returns>The reference term.</returns>
		/// <param name="concept">Concept.</param>
		/// <param name="codeSystemOid">Code system oid.</param>
		public OpenIZ.Core.Model.DataTypes.ReferenceTerm GetReferenceTerm (OpenIZ.Core.Model.DataTypes.Concept concept, string codeSystemOid)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<CodeSystem>> ();
			var codeSystem = persistenceService.Query (o => o.Oid == codeSystemOid).FirstOrDefault ();
			if (codeSystem == null)
				return null;
			return concept.ReferenceTerms.Find (o => o.RelationshipTypeKey == ConceptRelationshipTypeKeys.SameAs &&
				o.ReferenceTerm.CodeSystemKey == codeSystem.Key).ReferenceTerm;
		}
		/// <summary>
		/// Get the specified concept
		/// </summary>
		/// <returns>The concept.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="versionId">Version identifier.</param>
		public OpenIZ.Core.Model.DataTypes.Concept GetConcept (Guid id, Guid versionId)
		{
			var persistenceService = ApplicationContext.Current.GetService<IDataPersistenceService<Concept>> ();
			return persistenceService.Get (id);
		}
		#endregion
		
	}
}

