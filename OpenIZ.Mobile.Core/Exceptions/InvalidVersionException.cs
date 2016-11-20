using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Exceptions
{
    /// <summary>
    /// Invalid version exception
    /// </summary>
    public class InvalidVersionException : Exception
    {

        public InvalidVersionException(Version expected, Version actual) : base ($"Invalid version. Found {actual} but expected {expected}")
        {

        }
    }
}
