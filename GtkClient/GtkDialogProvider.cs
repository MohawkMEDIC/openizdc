using System;
using DisconnectedClient.Core;
using Gtk;

namespace GtkClient
{
	/// <summary>
	/// Gtk dialog provider.
	/// </summary>
	public class GtkDialogProvider : IDialogProvider
	{

		public bool Confirm(String text, String title) {
			bool? result = null;
			Application.Invoke((o,e) => {
				var md = new MessageDialog (null, DialogFlags.Modal, MessageType.Question, ButtonsType.OkCancel, text);
				result = (ResponseType)md.Run () == ResponseType.Ok;
				md.Destroy ();
			});
			while (!result.HasValue)
				Application.RunIteration (false);
			return result.Value;
		}

		public void Alert(String text) {
			ResponseType? result = null;
			Application.Invoke ((o, e) => {
				var md = new MessageDialog (null, DialogFlags.Modal, MessageType.Other, ButtonsType.Close, text);
				result = (ResponseType)md.Run ();
				md.Destroy ();
			});
			while (!result.HasValue)
				Application.RunIteration (false);
		}
	}
}

