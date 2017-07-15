using System;
using Gtk;

namespace GtkClient
{

	public class MessageBox : Gtk.Dialog
	{

		public MessageBox (String title, String message)
			: base(title, null, DialogFlags.DestroyWithParent | DialogFlags.Modal, "Ok", ResponseType.Ok)
		{
			this.VBox.Add (new Label(message));
			this.ShowAll ();
		}

		public ResponseType Show() {
			return (ResponseType)this.Run ();
		}

		public override void Dispose ()
		{
			this.Destroy ();
		}

		public static ResponseType Show(String message, String title) {
			using (var msg = new MessageBox (title, message))
				return msg.Show ();
		}
	}

	public class ConfirmBox : Gtk.Dialog
	{

		public ConfirmBox (String title, String message)
			: base(title, null, DialogFlags.DestroyWithParent | DialogFlags.Modal, "Ok", ResponseType.Ok, "Cancel", ResponseType.Cancel)
		{
			this.VBox.Add (new Label(message));
			this.ShowAll ();
		}

		public ResponseType Show() {
			return (ResponseType)this.Run ();
		}

		public override void Dispose ()
		{
			this.Destroy ();
		}

		public static ResponseType Show(String message, String title) {
			using (var msg = new ConfirmBox (title, message))
				return msg.Show ();
		}

	}
}

