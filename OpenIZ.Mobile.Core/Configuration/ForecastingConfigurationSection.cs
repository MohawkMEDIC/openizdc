using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenIZ.Mobile.Core.Configuration
{
    /// <summary>
    /// Forecasting configuration section
    /// </summary>
    [XmlType(nameof(ForecastingConfigurationSection), Namespace = "http://openiz.org/mobile/configuration")]
    public class ForecastingConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// Represents protocol directories
        /// </summary>
        [XmlElement("protocolDirectory")]
        public String ProtocolSourceDirectory { get; set; }
    }
}
