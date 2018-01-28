using System;
using GLib;
using WebKit;
using Gtk;
using System.Runtime.InteropServices;

namespace GtkClient
{

	/// <summary>
	/// Signal arguments
	/// </summary>
	public class ConsoleSignalArgs : SignalArgs {

		public ConsoleSignalArgs ()
		{
			
		}
		/// <summary>
		/// Gets the web frame
		/// </summary>
		/// <value>The frame.</value>
		public String Source {
			get {
				return (String)base.Args [2];
			}
		}

		/// <summary>
		/// Gets the web frame
		/// </summary>
		/// <value>The frame.</value>
		public Int32 Line {
			get {
				return (Int32)base.Args [1];
			}
		}

		/// <summary>
		/// Gets the web frame
		/// </summary>
		/// <value>The frame.</value>
		public String Message {
			get {
				return (String)base.Args [0];
			}
		}
	}

	public delegate void ConsoleSignalHandler(object o, ConsoleSignalArgs e);

	/// <summary>
	/// OpenIZ DC Extensions to WebView
	/// </summary>
	public class OpenIZWebView : WebKit.WebView
	{

		/// <summary>
		/// Occurs when close is requested
		/// </summary>
		public event EventHandler<SignalArgs> Closed {
			add {
				Signal signal = Signal.Lookup(this, "close-web-view", typeof(SignalArgs));
				signal.AddDelegate(value);
			}
			remove {
				Signal signal = Signal.Lookup(this, "close-web-view", typeof(SignalArgs));
				signal.RemoveDelegate(value);
			}
		}

		/// <summary>
		/// Occurs when close is requested
		/// </summary>
		public event ConsoleSignalHandler ConsoleMessage {
			add {
				Signal signal = Signal.Lookup(this, "console-message", typeof(ConsoleSignalArgs));
				signal.AddDelegate(value);
			}
			remove {
				Signal signal = Signal.Lookup(this, "console-message", typeof(ConsoleSignalArgs));
				signal.RemoveDelegate(value);
			}
		}


	}
}

