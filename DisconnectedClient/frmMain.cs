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
    public partial class frmDisconnectedClient : Form
    {
        public frmDisconnectedClient()
        {
            
            InitializeComponent();
            var asm = typeof(OpenIZ.Mobile.Core.ApplicationContext).Assembly;
            lblVersion.Text = $"v.{asm.GetName().Version} {asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}";

            Action<ApplicationProgressEventArgs> updateUi = (e) =>
            {
                lblProgress.Text = e.ProgressText;
                if (e.Progress >= 0 && e.Progress <= 1)
                {
                    pgMain.Style = ProgressBarStyle.Continuous;

                    pgMain.Value = (int)(e.Progress * 100);
                }
                else
                    pgMain.Style = ProgressBarStyle.Marquee;
            };

            XamarinApplicationContext.ProgressChanged += (o, e) =>
            {
                this.Invoke(updateUi, e);
            };
        }
    }
}
