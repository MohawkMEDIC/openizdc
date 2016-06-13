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
        private IDataPersistenceService<ConceptSet> m_persister = ApplicationContext.Current.GetService<IDataPersistenceService<ConceptSet>>();

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
                var linqQuery = parser.BuildLinqExpression<ConceptSet>(NameValueCollection.ParseQueryString(imsiQueryString));

                // Perform the query
                int totalResults = 0;
                var results = this.m_persister.Query(linqQuery, offset, count, out totalResults);

                // Return bundle
                OpenIZ.Core.Model.Collection.Bundle.CreateBundle(results, totalResults, offset);
                return JniUtil.ToJson(new OpenIZ.Core.Model.Collection.Bundle()
                {
                    Item = new List<OpenIZ.Core.Model.IdentifiedData>(results.OfType<IdentifiedData>()),
                    Count = results.Count(),
                    TotalResults = totalResults,
                    Offset = offset
                });
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing query {0}: {1}", imsiQueryString, e);
                return "err_general";
            }
        }
	}
}

