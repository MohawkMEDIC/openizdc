using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenIZ.Mobile.Core.Xamarin.Services.Model
{
    /// <summary>
    /// Menu information
    /// </summary>
    [JsonObject]
    public class MenuInformation
    {


        /// <summary>
        /// Get or sets the menu
        /// </summary>
        [JsonProperty("menu")]
        public List<MenuInformation> Menu { get; set; }

        /// <summary>
        /// Icon text
        /// </summary>
        [JsonProperty("icon")]
        public String Icon { get; set; }

        /// <summary>
        /// Text for the menu item
        /// </summary>
        [JsonProperty("text")]
        public String Text { get; set; }

        /// <summary>
        /// Action
        /// </summary>
        [JsonProperty("action")]
        public String Action { get; set; }
    }
}
