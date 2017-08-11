using System;

namespace DisconnectedClient.Core
{
	/// <summary>
	/// Represents a dialog box provider to handle differences between GTK+ and WinForms dialogs
	/// </summary>
	public interface IDialogProvider
	{

		/// <summary>
		/// Confirm the specified text and title.
		/// </summary>
		/// <param name="text">Text.</param>
		/// <param name="title">Title.</param>
		bool Confirm(String text, String title);

		/// <summary>
		/// Alert the specified text.
		/// </summary>
		/// <param name="text">Text.</param>
		void Alert(String text);

	}
}

