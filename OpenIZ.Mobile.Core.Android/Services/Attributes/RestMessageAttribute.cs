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
    /// REST Message format
    /// </summary>
    public enum RestMessageFormat
    {
        Json,
        Xml,
        FormData
    }

    /// <summary>
    /// REST message format attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
    public class RestMessageAttribute : Attribute
    {

        /// <summary>
        /// REST message format ctor
        /// </summary>
        /// <param name="format"></param>
        public RestMessageAttribute(RestMessageFormat format)
        {
            this.MessageFormat = format;
        }

        /// <summary>
        /// Message format
        /// </summary>
        public RestMessageFormat MessageFormat { get; set; }

    }
}