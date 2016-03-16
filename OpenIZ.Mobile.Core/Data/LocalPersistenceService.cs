using System;

using System.Linq;
using System.Reflection;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Diagnostics;

namespace OpenIZ.Mobile.Core.Data
{
	/// <summary>
	/// Represents a dummy service which just adds the persistence services to the context
	/// </summary>
	public class LocalPersistenceService
	{
		// Tracer
		private Tracer m_tracer = Tracer.GetTracer(typeof(LocalPersistenceService));

		public LocalPersistenceService ()
		{
			var appSection = ApplicationContext.Current.Configuration.GetSection<ApplicationConfigurationSection> ();

			// Iterate the persistence services
			foreach(var t in typeof(LocalPersistenceService).GetTypeInfo().Assembly.ExportedTypes.Where(o=>o.Namespace == "OpenIZ.Mobile.Core.Data.Persistence" && !o.GetTypeInfo().IsAbstract))
			{
				try
				{
					this.m_tracer.TraceVerbose ("Loading {0}...", t.AssemblyQualifiedName);
					appSection.Services.Add (Activator.CreateInstance (t));
				}
				catch(Exception e) {
					this.m_tracer.TraceError ("Error adding service {0} : {1}", t.AssemblyQualifiedName, e); 
				}
			}
		}
	}
}

