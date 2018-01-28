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
using DisconnectedClient.Core;
using System.Windows.Forms;
using CefSharp;

namespace DisconnectedClient
{
	/// <summary>
	/// Dialog provider
	/// </summary>
	public class WinFormsDialogProvider : IDialogProvider, IJsDialogHandler
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

        /// <summary>
        /// Chrome is showing a dialog
        /// </summary>
        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType, string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            switch(dialogType)
            {
                case CefJsDialogType.Alert:
                    this.Alert(messageText);
                    //suppressMessage = true;
                    callback.Continue(true);
                    break;
                case CefJsDialogType.Confirm:
                    //suppressMessage = true;
                    var retVal = this.Confirm(messageText, "Confirm");
                    callback.Continue(retVal);
                    break;
            }
            return true;

        }

        public bool OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            return false;
        }

        public void OnResetDialogState(IWebBrowser browserControl, IBrowser browser)
        {
        }

        public void OnDialogClosed(IWebBrowser browserControl, IBrowser browser)
        {
        }
    }
}

