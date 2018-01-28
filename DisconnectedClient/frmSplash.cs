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
using OpenIZ.Mobile.Core;
using OpenIZ.Mobile.Core.Xamarin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DisconnectedClient
{
    public partial class frmSplash : Form
    {

        EventHandler<ApplicationProgressEventArgs> m_progressHandler;

        public frmSplash()
        {
            InitializeComponent();
        }

        private void frmSplash_Load(object sender, EventArgs e)
        {
            lblVersion.Text = String.Format("v.{0} - {1}", Assembly.GetEntryAssembly().GetName().Version, Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion);

            Action<ApplicationProgressEventArgs> updateUi = (ev) =>
            {
                lblProgress.Text = String.Format("{0} ({1:#0}%)", ev.ProgressText, ev.Progress * 100);
                Application.DoEvents();
            };

            this.m_progressHandler = (o, ev) =>
            {
                try
                {
                    this.Invoke(updateUi, ev);
                }
                catch { }
            };
            XamarinApplicationContext.ProgressChanged += this.m_progressHandler;
        }

        private void frmSplash_FormClosing(object sender, FormClosingEventArgs e)
        {
            XamarinApplicationContext.ProgressChanged -= this.m_progressHandler;
        }
    }
}
