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

namespace OpenIZ.Mobile.Core.Android.Services.Attributes
{
    /// <summary>
    /// Annotates an operation on a rest service
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RestOperationAttribute : Attribute
    {
        /// <summary>
        /// Rest operation
        /// </summary>
        public RestOperationAttribute()
        {
                
        }

        /// <summary>
        /// Gets or sets the fault provider
        /// </summary>
        public string FaultProvider { get; set; }

        /// <summary>
        /// Filter of the HTTP method
        /// </summary>
        public string Method { get; set; }
        
        /// <summary>
        /// URL template for the operation
        /// </summary>
        public String UriPath { get; set; }


    }
}