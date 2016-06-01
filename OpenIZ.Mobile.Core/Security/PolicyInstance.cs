using OpenIZ.Core.Model.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Security
{
    /// <summary>
    /// Represents a policy instance 
    /// </summary>
    public class PolicyInstance : IPolicyInstance
    {
        /// <summary>
        /// Constructs a new instance of the policy instance
        /// </summary>
        public PolicyInstance(IPolicy policy, PolicyGrantType rule)
        {
            this.Policy = policy;
            this.Rule = rule;
        }

        /// <summary>
        /// Gets the policy to which the instance applies
        /// </summary>
        public IPolicy Policy
        {
            get; private set;
        }

        /// <summary>
        /// Gets the rule or, rather, the enforcement type
        /// </summary>
        public PolicyGrantType Rule
        {
            get;private set;
        }

        /// <summary>
        /// Gets or sets the policy securable
        /// </summary>
        public object Securable
        {
            get;private set;
        }
    }
}
