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
using OpenIZ.Core.Services;
using OpenIZ.Core.Model.Collection;
using OpenIZ.Core.Applets.ViewModel;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
	/// <summary>
	/// Represents the concept service bridge
	/// </summary>
	public class ConceptServiceBridge : Java.Lang.Object
	{

        // Concept repository service
        private IConceptRepositoryService m_conceptService = ApplicationContext.Current.GetService<IConceptRepositoryService>();

        // Local variables
        private Tracer m_tracer = Tracer.GetTracer(typeof(ConceptServiceBridge));


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
                var request = NameValueCollection.ParseQueryString(imsiQueryString);
                var linqQuery = QueryExpressionParser.BuildLinqExpression<Concept>(request);

                // Perform the query
                int totalResults = 0;
                var results = this.m_conceptService.FindConcepts(linqQuery, offset, count, out totalResults);

                // Expand properties
                foreach (var itm in results)
                    JniUtil.ExpandProperties(itm, request);

                // Return bundle
                var retVal = new Bundle();
                retVal.Item.AddRange(results);
                retVal.TotalResults = totalResults;
                retVal.Count = retVal.Item.Count;
                retVal.Offset = offset;
                return JsonViewModelSerializer.Serialize(retVal);
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
                var request = NameValueCollection.ParseQueryString(imsiQueryString);
                var linqQuery = QueryExpressionParser.BuildLinqExpression<ConceptSet>(request);

                // Perform the query
                int totalResults = 0;
                var results = this.m_conceptService.FindConceptSets(linqQuery, offset, count, out totalResults);


                // Expand properties
                foreach (var itm in results)
                    JniUtil.ExpandProperties(itm, request);

                // Return bundle
                var retVal = new Bundle();
                retVal.Item.AddRange(results);
                retVal.TotalResults = totalResults;
                retVal.Count = retVal.Item.Count;
                retVal.Offset = offset;
                return JsonViewModelSerializer.Serialize(retVal);
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError("Error executing query {0}: {1}", imsiQueryString, e);
                return "err_general";
            }
        }
	}
}

