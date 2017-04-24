using OpenIZ.Core.Model.Query;
using OpenIZ.Mobile.Core;
using OpenIZ.Mobile.Core.Xamarin.Services.Attributes;
using OpenIZ.Mobile.Core.Xamarin.Services.Model;
using OpenIZ.Mobile.Reporting;
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
            return ApplicationContext.Current.GetService<IReportRepository>().Reports;
        }

        /// <summary>
        /// Get the output of a report.
        /// </summary>
        [RestOperation(Method = "GET", UriPath = "/report.htm", FaultProvider = nameof(RisiFaultHandler))]
        [return: RestMessage(RestMessageFormat.Json)]
        public byte[] ExecuteReport()
        {
            String _view = MiniImsServer.CurrentContext.Request.QueryString["_view"],
                _name = MiniImsServer.CurrentContext.Request.QueryString["_name"];

            // Name and view
            if (!String.IsNullOrEmpty(_view) && !String.IsNullOrEmpty(_name))
                return ApplicationContext.Current.GetService<ReportExecutor>().RenderReport(_name, _view, NameValueCollection.ParseQueryString(MiniImsServer.CurrentContext.Request.Url.Query).ToDictionary(o => o.Key, o => (Object)o.Value));
            else
                throw new ArgumentNullException(nameof(_view));
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
