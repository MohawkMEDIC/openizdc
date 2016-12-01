using OpenIZ.BusinessRules.JavaScript;
using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OpenIZ.Mobile.Core.Xamarin.Resources;
using OpenIZ.Core.Diagnostics;
using OpenIZ.Core.Applets.ViewModel.Description;

namespace OpenIZ.Mobile.Core.Xamarin.Rules
{
    /// <summary>
    /// Business rules service which adds javascript files
    /// </summary>
    public class BusinessRulesDaemonService : IDaemonService, IDataReferenceResolver
    {

        private Tracer m_tracer = Tracer.GetTracer(typeof(BusinessRulesDaemonService));
        /// <summary>
        /// Indicates whether the service is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Events
        /// </summary>
        public event EventHandler Started;
        public event EventHandler Starting;
        public event EventHandler Stopped;
        public event EventHandler Stopping;

        /// <summary>
        /// Resolve asset
        /// </summary>
        public Stream Resolve(string reference)
        {
            var itm = XamarinApplicationContext.Current.LoadedApplets.SelectMany(a => a.Assets).FirstOrDefault(a => a.Name.EndsWith(reference));
            if (itm == null)
                return null;
            return new MemoryStream(XamarinApplicationContext.Current.LoadedApplets.RenderAssetContent(itm));
        }

        /// <summary>
        /// Start the service which will register items with the business rules handler
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);
            ApplicationContext.Current.Started += (o,e) =>
            {
                try
                {
                    foreach (var itm in XamarinApplicationContext.Current.LoadedApplets.SelectMany(a => a.Assets).Where(a => a.Name.StartsWith("rules/")))
                        using (StreamReader sr = new StreamReader(new MemoryStream(XamarinApplicationContext.Current.LoadedApplets.RenderAssetContent(itm))))
                        {
                            BusinessRules.JavaScript.JavascriptBusinessRulesEngine.Current.AddRules(sr);
                            this.m_tracer.TraceInfo("Added rules from {0}", itm.Name);
                        }
                    BusinessRules.JavaScript.JavascriptBusinessRulesEngine.Current.Bridge.Serializer.ViewModel = ViewModelDescription.Load(typeof(BusinessRulesDaemonService).Assembly.GetManifestResourceStream("OpenIZ.Mobile.Core.Xamarin.Resources.ViewModel.xml"));
                    BusinessRules.JavaScript.JavascriptBusinessRulesEngine.Current.Bridge.Serializer.LoadSerializerAssembly(typeof(OpenIZ.Core.Model.Json.Formatter.ActExtensionViewModelSerializer).Assembly);

                }
                catch (Exception ex)
                {
                    this.m_tracer.TraceError("Error on startup: {0}", ex);
                    throw new InvalidOperationException(Strings.err_startup_error, ex);
                }
            };
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stopping
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);
            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
