using System;
using OpenIZ.Mobile.Core.Configuration;
using OpenIZ.Mobile.Core.Security;
using System.Security.Principal;
using OpenIZ.Mobile.Core.Services;

namespace OpenIZ.Mobile.Core
{
	/// <summary>
	/// Application context.
	/// </summary>
	public abstract class ApplicationContext
	{

		// Context singleton
		private static ApplicationContext s_context;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenIZ.Mobile.Core.ApplicationContext"/> class.
		/// </summary>
		public ApplicationContext ()
		{
			this.PolicyDecisionService = new LocalPolicyDecisionService ();
			this.PolicyInformationService = new LocalPolicyInformationService ();

		}

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
		public IPolicyInformationService PolicyInformationService { get; private set; }
		/// <summary>
		/// Gets the policy decision service.
		/// </summary>
		/// <value>The policy decision service.</value>
		public IPolicyDecisionService PolicyDecisionService { get; private set; }
		/// <summary>
		/// Gets the identity provider service.
		/// </summary>
		/// <value>The identity provider service.</value>
		public IIdentityProviderService IdentityProviderService { get; private set; }
		/// <summary>
		/// Gets the role provider service.
		/// </summary>
		/// <value>The role provider service.</value>
		public IRoleProviderService RoleProviderService { get; private set; }
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

	}
}

