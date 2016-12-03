using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Inspectors
{
    public class Base64TextDecoder : DataInspectorBase
    {
        public override string Name
        {
            get
            {
                return "Base64 Text Decoder";
            }
        }

        public override string Inspect(string source)
        {
            try
            {
                var bytes = Convert.FromBase64String(source);
                return Encoding.UTF8.GetString(bytes);   
            }
            catch
            {
                return "Invalid Data";
            }
        }
    }


    public class Base6DecodeInspector : DataInspectorBase
    {
        public override string Name
        {
            get
            {
                return "Base64 Binary Decoder";
            }
        }
        
        public override string Inspect(string source)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                var bytes = Convert.FromBase64String(source);
                sb.AppendFormat("{0} bytes\r\n", bytes.Length);
                for (int i = 0; i < bytes.Length; i += 8)
                {
                    foreach (var e in Enumerable.Range(i, 8))
                    {
                        if (e < bytes.Length)
                            sb.AppendFormat("0x{0:X2} ", bytes[e]);
                        else
                            sb.AppendFormat("    ");
                        if (e % 4 == 3) sb.Append("\t");
                    }
                    sb.AppendFormat("{0}\r\n", Encoding.UTF8.GetString(bytes, i, bytes.Length - i < 8 ? bytes.Length - i : 8));
                }
                return sb.ToString();
            }
            catch
            {
                return "Invalid Data";
            }
        }
    }
}
