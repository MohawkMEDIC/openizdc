using System;

namespace GtkClient
{
	public class WebkitSettings : WebKit.WebSettings
	{
		public WebkitSettings (String userAgent)
		{
			base.SetProperty ("user-agent", new GLib.Value (userAgent));

		}
	}
}

