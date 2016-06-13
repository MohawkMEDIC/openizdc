using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OpenIZ.Mobile.Core.Diagnostics;
using OpenIZ.Mobile.Core.Services;
using OpenIZ.Core.Model.Roles;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Model;
using Android.Webkit;
using Java.Interop;

namespace OpenIZ.Mobile.Core.Android.AppletEngine.JNI
{
    /// <summary>
    /// Represents patient function bridges
    /// </summary>
    public class PatientFunctionBridge : Java.Lang.Object
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(PatientFunctionBridge));

        // Patient persister
        private IDataPersistenceService<Patient> m_persister = ApplicationContext.Current.GetService<IDataPersistenceService<Patient>>();

        /// <summary>
        /// IMSI Patient Object
        /// </summary>
        public string Insert(String imsiPatientObject)
        {
            try
            {
                var patient = JniUtil.FromJson<Patient>(imsiPatientObject);
                if (patient == null)
                    throw new ArgumentException("Invalid type", nameof(imsiPatientObject));
                var retVal = this.m_persister.Insert(patient);
                return JniUtil.ToJson(retVal);
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error registering patient {0}: {1}", imsiPatientObject, e);
                return "err_general";
            }
        }

        /// <summary>
        /// Search for a patient using an imsi query string
        /// </summary>
        [JavascriptInterface]
        [Export]
        public string Search(String imsiQueryString, int offset, int count)
        {
            try
            {
                // Parse the queyr
                QueryExpressionParser parser = new QueryExpressionParser();
                var linqQuery = parser.BuildLinqExpression<Patient>(NameValueCollection.ParseQueryString(imsiQueryString));

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
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error executing query {0}: {1}", imsiQueryString, e);
                return "err_general";
            }
        }


    }
}