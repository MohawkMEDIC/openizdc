using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Inspectors
{
    /// <summary>
    /// Represents a text data inspector
    /// </summary>
    public class TextDataInspector : DataInspectorBase
    {
        public override string Name
        {
            get
            {
                return "Text";
            }
        }

        /// <summary>
        /// Inspect
        /// </summary>
        public override string Inspect(string source)
        {
            return source;
        }

        
    }
}
