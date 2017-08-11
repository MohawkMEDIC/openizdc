using System;
using DisconnectedClient.Core;
using System.Windows.Forms;

namespace DisconnectedClient
{
	/// <summary>
	/// Dialog provider
	/// </summary>
	public class WinFormsDialogProvider : IDialogProvider
	{

		/// <summary>
		/// Confirm the specified text and title.
		/// </summary>
		public bool Confirm(String text, String title) {
			return MessageBox.Show (text, title, MessageBoxButtons.OKCancel) == DialogResult.OK;
		}

		/// <summary>
		/// Alert the specified text
		/// </summary>
		public void Alert(String text) {
			MessageBox.Show (text);
		}
	}
}

