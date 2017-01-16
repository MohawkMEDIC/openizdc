using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Inspectors
{
    public class CompressedDataInspector : DataInspectorBase
    {
        public override string Name
        {
            get
            {
                return "GZIP Decoder";
            }
        }

        public override string Inspect(string source)
        {
            try
            {
                var bytes = Convert.FromBase64String(source);
                using (var ms = new MemoryStream(bytes))
                {
                    try
                    {
                        using (GZipStream gzs = new GZipStream(ms, CompressionMode.Decompress))
                        using (StreamReader sr = new StreamReader(gzs))
                            return sr.ReadToEnd();
                    }
                    catch 
                    {
                        using (DeflateStream gzs = new DeflateStream(ms, CompressionMode.Decompress))
                        using (StreamReader sr = new StreamReader(gzs))
                            return sr.ReadToEnd();
                    }
                }
            }
            catch
            {
                return "Invalid Data";
            }
        }
    }
}
