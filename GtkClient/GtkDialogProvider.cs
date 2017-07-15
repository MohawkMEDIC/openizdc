using System;
using DisconnectedClient.Core;

namespace GtkClient
{
	/// <summary>
	/// Gtk dialog provider.
	/// </summary>
	public class GtkDialogProvider : IDialogProvider
	{

		public bool Confirm(String text, String title) {
			return ConfirmBox.Show (text, title) == Gtk.ResponseType.Ok;
		}

		public void Alert(String text) {
			MessageBox.Show (text, String.Empty);
		}
	}
}

