using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Attributes
{
    /// <summary>
    /// Represents a demand for a particular permission to perform the action
    /// </summary>
    public class DemandAttribute : System.Attribute
    {

        /// <summary>
        /// Demand attribute
        /// </summary>
        public DemandAttribute(String policyId)
        {
            this.PolicyId = policyId;
        }

        /// <summary>
        /// Policy identifier for demand
        /// </summary>
        public String PolicyId { get; set; }

    }
}
