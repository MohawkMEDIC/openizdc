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
 * Date: 2016-6-14
 */
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
using OpenIZ.Core.Applets.ViewModel;
using OpenIZ.Mobile.Core.Android.Resources;

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
        [Export]
        [JavascriptInterface]
        public string Insert(String imsiPatientObject)
        {
            try
            {
                var patient = JsonViewModelSerializer.DeSerialize<Patient>(imsiPatientObject);
                if (patient == null)
                    throw new ArgumentException(Strings.err_invalid_argumentType, nameof(imsiPatientObject));
                patient = this.m_persister.Insert(patient);
                var retVal = JsonViewModelSerializer.Serialize(patient);
                return retVal;
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
                var request = NameValueCollection.ParseQueryString(imsiQueryString);
                var linqQuery = QueryExpressionParser.BuildLinqExpression<Patient>(request);

                // Perform the query
                int totalResults = 0;
                var results = this.m_persister.Query(linqQuery, offset, count, out totalResults);
                //foreach (var itm in results)
                //    JniUtil.ExpandProperties(itm, request);

                // Return bundle
                var bundle = new OpenIZ.Core.Model.Collection.Bundle();
                bundle.Item.AddRange(results);
                bundle.TotalResults = totalResults;
                bundle.Count = results.Count();
                bundle.Offset = offset;
                var retVal = JsonViewModelSerializer.Serialize(bundle);
                return retVal;
            }
            catch(Exception e)
            {
                this.m_tracer.TraceError("Error executing query {0}: {1}", imsiQueryString, e);
                return "err_general";
            }
        }


    }
}