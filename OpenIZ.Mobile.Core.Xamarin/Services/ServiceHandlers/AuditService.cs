/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-9-1
 */
using MARC.HI.EHRS.SVC.Auditing.Data;
using OpenIZ.Core.Model.AMI.Security;
using OpenIZ.Core.Model.Query;
using OpenIZ.Core.Services;
using OpenIZ.Mobile.Core.Security;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents the audit rest service
    /// </summary>
    [RestService("/__audit")]
    public class AuditService
    {

        /// <summary>
        /// Get audits
        /// </summary>
        [RestOperation(FaultProvider = nameof(AuditFaultProvider), Method = "GET", UriPath = "/audit")]
        [Demand(PolicyIdentifiers.AccessClientAdministrativeFunction)]
        [return: RestMessage(RestMessageFormat.Json)]
        public AmiCollection<AuditInfo> GetAudits()
        {
            var auditRepository = ApplicationContext.Current.GetService<IAuditRepositoryService>();
            var search = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query);

            if (search.ContainsKey("_id"))
            {
                // Force load from DB
                var retVal = auditRepository.Get(search["_id"][0]);
                return new AmiCollection<AuditInfo>(new AuditInfo[] { new AuditInfo(retVal) });
            }
            else
            {

                int totalResults = 0,
                       offset = search.ContainsKey("_offset") ? Int32.Parse(search["_offset"][0]) : 0,
                       count = search.ContainsKey("_count") ? Int32.Parse(search["_count"][0]) : 100;

                var results = auditRepository.Find(QueryExpressionParser.BuildLinqExpression<AuditData>(search), offset, count, out totalResults);
                return new AmiCollection<AuditInfo>(results.Select(o => new AuditInfo(o)))
                {
                    Size = totalResults,
                    Offset = offset
                };
            }
        }

        /// <summary>
        /// Fault provider
        /// </summary>
        public ErrorResult AuditFaultProvider(Exception e)
        {
            return new ErrorResult(e);
        }
    }
}
