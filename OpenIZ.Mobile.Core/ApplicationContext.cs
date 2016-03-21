using System;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Security;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Services;
using System.Collections.Generic;
using System.Reflection;
using OpenIZ.Core.Model.Security;
using OpenIZ.Core.Model.EntityLoader;

namespace OpenIZ.Mobile.Core
{
	/// <summary>
	/// Application context.
	/// </summary>
	public abstract class ApplicationContext : IServiceProvider
	{

		// Context singleton
		private static ApplicationContext s_context;

		// Providers
		private List<Object> m_providers;

		// A cache of already found providers
		private Dictionary<Type, Object> m_cache = new Dictionary<Type, object>();

		// Lock object
		private Object m_lockObject = new object();

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.ApplicationContext"/> class.
		/// </summary>
		public ApplicationContext ()
		{
		}

		#region IServiceProvider implementation

		/// <summary>
		/// Gets the service.
		/// </summary>
		/// <returns>The service.</returns>
		/// <typeparam name="TService">The 1st type parameter.</typeparam>
		public TService GetService<TService>()
		{
			return (TService)this.GetService (typeof(TService));
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <returns>The service.</returns>
		/// <param name="serviceType">Service type.</param>
		public object GetService (Type serviceType)
		{
			
			Object candidateService = null;
			if (!this.m_cache.TryGetValue (serviceType, out candidateService)) {
				ApplicationConfigurationSection appSection = this.Configuration.GetSection<ApplicationConfigurationSection> ();
				candidateService = appSection.Services.Find (o => serviceType.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo()));
				if (candidateService != null)
					lock (this.m_lockObject)
						this.m_cache.Add (serviceType, candidateService);
			}
			return candidateService;
		}

		#endregion

		/// <summary>
		/// Gets the current application context
		/// </summary>
		/// <value>The current.</value>
		public static ApplicationContext Current
		{
			get { return s_context; }
			set {
				if (s_context == null || value == null)
					s_context = value;
				else
					throw new InvalidOperationException ("Application context already set");
			}
		}

		/// <summary>
		/// Gets the policy information service.
		/// </summary>
		/// <value>The policy information service.</value>
		public IPolicyInformationService PolicyInformationService { get { return this.GetService(typeof(IPolicyInformationService)) as IPolicyInformationService; } }
		/// <summary>
		/// Gets the policy decision service.
		/// </summary>
		/// <value>The policy decision service.</value>
		public IPolicyDecisionService PolicyDecisionService { get { return this.GetService(typeof(IPolicyDecisionService)) as IPolicyDecisionService; } }
		/// <summary>
		/// Gets the identity provider service.
		/// </summary>
		/// <value>The identity provider service.</value>
		public IIdentityProviderService IdentityProviderService { get { return this.GetService(typeof(IIdentityProviderService)) as IIdentityProviderService; } }
		/// <summary>
		/// Gets the role provider service.
		/// </summary>
		/// <value>The role provider service.</value>
		public IRoleProviderService RoleProviderService { get { return this.GetService(typeof(IRoleProviderService)) as IRoleProviderService; } }
		/// <summary>
		/// Gets or sets the principal.
		/// </summary>
		/// <value>The principal.</value>
		public IPrincipal Principal { get; protected set; }
		/// <summary>
		/// Gets the configuration.
		/// </summary>
		/// <value>The configuration.</value>
		public abstract OpenIZConfiguration Configuration { get; }
		/// <summary>
		/// Gets the application information for the currently running application.
		/// </summary>
		/// <value>The application.</value>
		public abstract SecurityApplication Application { get; }
		/// <summary>
		/// Gets the device information for the currently running device
		/// </summary>
		/// <value>The device.</value>
		public abstract SecurityDevice Device { get; }
	}
}

