using OpenIZ.Mobile.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace OpenIZ.Mobile.Core.Xamarin.Services
{
    /// <summary>
    /// A compression stream service supporting deflate
    /// </summary>
    public class DeflateStreamCompressionService : IStreamCompressionService
    {
        /// <summary>
        /// Get compression stream
        /// </summary>
        public Stream GetCompressionStream(Stream inner)
        {
            return new DeflateStream(inner, CompressionMode.Compress);
        }

        /// <summary>
        /// Get de-compression stream
        /// </summary>
        public Stream GetDeCompressionStream(Stream inner)
        {
            return new DeflateStream(inner, CompressionMode.Decompress);
        }
    }
}
