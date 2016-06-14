using System;
using Java.Interop;
using Android.Webkit;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Core.Model.DataTypes;
using OpenIZ.Core.Model.Query;
using System.Collections.Generic;
using System.Linq;
using OpenIZ.Core.Model;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	/// <summary>
	/// Represents the concept service bridge
	/// </summary>
	public class ConceptServiceBridge : Java.Lang.Object
	{

        // Local variables
        private Tracer m_tracer = Tracer.GetTracer(typeof(ConceptServiceBridge));
        private IDataPersistenceService<ConceptSet> m_setPersister = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();
        private IDataPersistenceService<Concept> m_conceptPersister= ApplicationContext.Current.GetService<IDataPersistenceService<Concept>>();


        /// <summary>
        /// Search for a concept set.
        /// </summary>
        /// <returns>The concept set.</returns>
        /// <param name="imsiQueryString">IMSI query to retrieve the concept set</param>
        [Export]
        [JavascriptInterface]
        public String SearchConcept(String imsiQueryString, int offset, int count)
        {
            try
            {
                // Parse the queyr
                QueryExpressionParser parser = new QueryExpressionParser();
                var request = NameValueCollection.ParseQueryString(imsiQueryString);
                var linqQuery = parser.BuildLinqExpression<Concept>(request);

                // Perform the query
                int totalResults = 0;
                var results = this.m_conceptPersister.Query(linqQuery, offset, count, out totalResults);

                // Expand properties
                foreach (var itm in results)
                    JniUtil.ExpandProperties(itm, request);

                // Return bundle
                var retVal = OpenIZ.Core.Model.Collection.Bundle.CreateBundle(results, totalResults, offset);
                return JniUtil.ToJson(retVal);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing query {0}: {1}", imsiQueryString, e);
                return "err_general";
            }
        }

        /// <summary>
        /// Search for a concept set.
        /// </summary>
        /// <returns>The concept set.</returns>
        /// <param name="imsiQueryString">IMSI query to retrieve the concept set</param>
        [Export]
		[JavascriptInterface]
		public String SearchConceptSet(String imsiQueryString, int offset, int count)
		{
            try
            {
                // Parse the queyr
                QueryExpressionParser parser = new QueryExpressionParser();
                var request = NameValueCollection.ParseQueryString(imsiQueryString);
                var linqQuery = parser.BuildLinqExpression<ConceptSet>(request);

                // Perform the query
                int totalResults = 0;
                var results = this.m_setPersister.Query(linqQuery, offset, count, out totalResults);


                // Expand properties
                foreach (var itm in results)
                    JniUtil.ExpandProperties(itm, request);

                // Return bundle
                var bundle = OpenIZ.Core.Model.Collection.Bundle.CreateBundle(results, totalResults, offset);
                string retVal = JniUtil.ToJson(bundle);
                return retVal;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing query {0}: {1}", imsiQueryString, e);
                return "err_general";
            }
        }
	}
}

