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
    /// REST service attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RestServiceAttribute : Attribute
    {
        /// <summary>
        /// REST Service attribute for base address
        /// </summary>
        public RestServiceAttribute(string baseAddress)
        {
            this.BaseAddress = baseAddress;
        }

        /// <summary>
        /// Gets or sets the base address of the service
        /// </summary>
        public String BaseAddress { get; set; }

    }
}