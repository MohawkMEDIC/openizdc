/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2017-9-1
 */
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

