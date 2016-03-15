using System;
using OpenIZ.Core.Model;
using OpenIZ.Core.Model.DataTypes;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OpenIZ.Mobile.Core.Services
{
	public interface IConceptService
	{

		/// <summary>
		/// Get the concept set by mnemonic
		/// </summary>
		ConceptSet GetConceptSet(string mnemonic);

		/// <summary>
		/// Get a concept by its mnemonic
		/// </summary>
		/// <param name="mnemonic">The concept mnemonic to get</param>
		Concept GetConcept(String mnemonic);

		/// <summary>
		/// Performs an arbirary query 
		/// </summary>
		/// <param name="query">The query to execute</param>
		IEnumerable<Concept> FindConcepts(Expression<Func<Concept, bool>> query);

		/// <summary>
		/// Finds a series of concepts by name
		/// </summary>
		IEnumerable<Concept> FindConceptsByName(String name, String language);

		/// <summary>
		/// Find a reference term by code system oid
		/// </summary>
		/// <param name="code">The code</param>
		/// <param name="codeSystemOid">The oid of the code system</param>
		IEnumerable<Concept> FindConceptsByReferenceTerm(String code, String codeSystemOid);

		/// <summary>
		/// Returns a value which indicates whether <paramref name="a"/> implies <paramref name="b"/>
		/// </summary>
		bool Implies(Concept a, Concept b);

		/// <summary>
		/// Returns true if the concept <paramref name="concept"/> is a member of set <paramref name="set"/>
		/// </summary>
		bool IsMember(ConceptSet set, Concept concept);

		/// <summary>
		/// Gets the specified reference term from <paramref name="concept"/> in <paramref name="codeSystemOid"/>
		/// </summary>
		ReferenceTerm GetReferenceTerm(Concept concept, String codeSystemOid);

		/// <summary>
		/// Gets the specified concept
		/// </summary>
		Concept GetConcept(Guid id, Guid versionId);

	}
}

