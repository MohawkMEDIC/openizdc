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
 * User: justi
 * Date: 2017-6-28
 */
using OpenIZ.Core.Model.Constants;
using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using OpenIZ.Mobile.Reporting;
using OpenIZ.Mobile.Reporting.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.ServiceHandlers
{
    /// <summary>
    /// Represents the RISI service
    /// </summary>
    [RestService("/__risi")]
    public class RisiService
    {

        /// <summary>
        /// Get a summary of all reports on the tablet
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/report", FaultProvider = nameof(RisiFaultHandler))]
        [return: RestMessage(RestMessageFormat.Json)]
        public IEnumerable GetSummary()
        {
            String _name = MiniImsServer.CurrentContext.Request.QueryString["_name"];
            if (!String.IsNullOrEmpty(_name))
                return new List<ReportDefinition>() { ApplicationContext.Current.GetService<IReportRepository>().GetReport(_name) };
            else
                return ApplicationContext.Current.GetService<IReportRepository>().Reports;
        }

        /// <summary>
        /// Get the output of a report.
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/report.htm", FaultProvider = nameof(RisiFaultHandler))]
        [return: RestMessage(RestMessageFormat.Raw)]
        public byte[] ExecuteReport()
        {
            String _view = MiniImsServer.CurrentContext.Request.QueryString["_view"],
                _name = MiniImsServer.CurrentContext.Request.QueryString["_name"];

            var query = this.GetQueryWithContext();

            // Name and view
            if (!String.IsNullOrEmpty(_view) && !String.IsNullOrEmpty(_name))
                return ApplicationContext.Current.GetService<ReportExecutor>().RenderReport(_name, _view, query);
            else
                throw new ArgumentNullException(nameof(_view));
        }

        /// <summary>
        /// Get query with context
        /// </summary>
        private IDictionary<String, Object> GetQueryWithContext()
        {
            var retVal = NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query).ToDictionary(o => o.Key, o => (Object)o.Value.FirstOrDefault());
            retVal.Add("Context_LocationId", AuthenticationContext.Current?.Session?.UserEntity?.Relationships.FirstOrDefault(o => o.Key == EntityRelationshipTypeKeys.DedicatedServiceDeliveryLocation)?.TargetEntityKey ??
                Guid.Parse(ApplicationContext.Current.Configuration.GetSection<SynchronizationConfigurationSection>().Facilities.FirstOrDefault()));
            retVal.Add("Context_UserEntityId", AuthenticationContext.Current.Session?.UserEntity?.Key);
            retVal.Add("Context_UserId", AuthenticationContext.Current.Principal?.Identity.Name);
            return retVal;
        }

        /// <summary>
        /// Get the output of a report.
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/data", FaultProvider = nameof(RisiFaultHandler))]
        [return: RestMessage(RestMessageFormat.Json)]
        public IEnumerable<dynamic> GetDataset()
        {
            
            String _report = MiniImsServer.CurrentContext.Request.QueryString["_report"],
                _name = MiniImsServer.CurrentContext.Request.QueryString["_name"];
            var query = this.GetQueryWithContext();

            // Name and view
            if (String.IsNullOrEmpty(_report) || String.IsNullOrEmpty(_name))
                throw new ArgumentNullException("Both report and name of dataset must be specified");
            else
                return ApplicationContext.Current.GetService<ReportExecutor>().RenderDataset(_report, _name, query);

        }
        /// <summary>
        /// RISI fault handler
        /// </summary>
        public ErrorResult RisiFaultHandler(Exception e)
        {
            return new ErrorResult()
            {
                Error = e.Message,
                ErrorDescription = e.InnerException?.Message,
                ErrorType = e.GetType().Name
            };
        }
    }
}
