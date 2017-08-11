using System;
using Gtk;
using DisconnectedClient;
using OpenIZ.Mobile.Core.Xamarin;
using OpenIZ.Mobile.Core.Xamarin.Services;
using OpenIZ.Mobile.Core.Xamarin.Security;
using System.Net;
using MohawkCollege.Util.Console.Parameters;
using System.IO;
using System.Net.Security;
using System.Collections.Generic;
using DisconnectedClient.Core;
using System.Reflection;
using OpenIZ.Mobile.Core;
using Pango;

namespace GtkClient
{
	class MainClass
	{
		// Trusted certificates
		private static List<String> s_trustedCerts = new List<string>();

		// Splash window
		private static Window s_splashWindow;
		private static Label m_statusLabel;

		/// <summary>
		/// Gets or sets the console parameters
		/// </summary>
		/// <value>The parameters.</value>
		public static ConsoleParameters Parameters {
			get;
			set;
		}

		public static void Main (string[] args)
		{

			Application.Init ();

			MainClass.Parameters = new ParameterParser<ConsoleParameters>().Parse(args);
			if (MainClass.Parameters.Debug)
				Console.WriteLine("Will start in debug mode...");
			if (MainClass.Parameters.Reset)
			{
				while (true) {
					Console.Write ("Are you sure you want to wipe all your data and configuration for the Disconnected Client? [Y/N]");
					var resp = Console.ReadKey ();
					switch (resp.Key) {
						case ConsoleKey.Y:
							var appData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDC");
							var cData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZDC");
							if (Directory.Exists(appData)) Directory.Delete(cData, true);
							if (Directory.Exists(appData)) Directory.Delete(appData, true);
							return;
						case ConsoleKey.N:
						case ConsoleKey.Escape:
							return;
					}
				}
			}
			String[] directory = {
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "OpenIZDC"),
				Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "OpenIZDC")
			};

			foreach (var dir in directory)
				if (!Directory.Exists(dir))
					Directory.CreateDirectory(dir);

			// Token validator
			TokenValidationManager.SymmetricKeyValidationCallback += (o, k, i) =>
			{
				return ConfirmBox.Show(String.Format("Trust issuer {0} with symmetric key?", i), "Token Validation Error") == ResponseType.Ok;
			};
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, error) =>
			{
				System.Diagnostics.Debugger.Break();
				if (certificate == null || chain == null)
					return false;
				else
				{
					var valid = s_trustedCerts.Contains(certificate.Subject);
					if (!valid && (chain.ChainStatus.Length > 0 || error != SslPolicyErrors.None))
					if (ConfirmBox.Show(String.Format("The remote certificate is not trusted. The error was {0}. The certificate is: \r\n{1}\r\nWould you like to temporarily trust this certificate?", error, certificate.Subject), "Certificate Error") == ResponseType.Cancel)
						return false;
					else
						s_trustedCerts.Add(certificate.Subject);

					return true;
					//isValid &= chain.ChainStatus.Length == 0;
				}
			};

			// Start up!!!
			try
			{

				s_splashWindow = new Window (WindowType.Toplevel);
				s_splashWindow.SetSizeRequest (640, 500);
				s_splashWindow.Decorated = false;
				s_splashWindow.SetPosition (WindowPosition.CenterAlways);
				s_splashWindow.Resizable = false;
				s_splashWindow.ModifyBg(StateType.Normal, new Gdk.Color(255, 255, 255));
				// Add image to the splash window
				var vbox = new VBox();

				var productLabel = new Label(Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title);
				var fontDescription = FontDescription.FromString("Sans Bold 24");
				productLabel.ModifyFont(fontDescription);

				var versionLabel = new Label(String.Format("{0} ({1})", Assembly.GetEntryAssembly().GetName().Version, Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion));
				var logoImage = new Image(Assembly.GetEntryAssembly(), "GtkClient.lib.logo_lg.png");
				logoImage.SetPadding(10, 10);
				m_statusLabel = new Label("Starting Up...");
				m_statusLabel.SetPadding(10, 10);

				vbox.PackStart(productLabel);
				vbox.PackStart(versionLabel);
				vbox.PackStart(logoImage);
				vbox.PackEnd(m_statusLabel);


				ApplicationContext.ProgressChanged += ApplicationContext_ProgressChanged;

				s_splashWindow.Add(vbox);

				s_splashWindow.ShowAll ();
				Application.Invoke(StartContext);
				Application.Run ();


			}
			catch (Exception e)
			{
				MessageBox.Show(e.ToString(), "Runtime Error");
			}
		}

		/// <summary>
		/// Progress changed for startup
		/// </summary>
		static void ApplicationContext_ProgressChanged (object sender, ApplicationProgressEventArgs e)
		{
			Application.Invoke((o,p)=> {
				m_statusLabel.Text = String.Format("{0} ({1:0%})", e.ProgressText, e.Progress);
			});
		}


		/// <summary>
		/// Start the IMS and do main program
		/// </summary>
		public static void StartContext(System.Object s, EventArgs ev) {

			bool started = false;
			EventHandler startHandler = (o, e) =>
			{
				started = true;
			};

			MainWindow main = null;
			if (!DcApplicationContext.StartContext(new GtkDialogProvider()))
			{
				DcApplicationContext.StartTemporary(new GtkDialogProvider());
				var minims = XamarinApplicationContext.Current.GetService<MiniImsServer>();

				DateTime start = new DateTime();

				if (!minims.IsRunning)
				{
					minims.Started += startHandler;
					while (!started && DateTime.Now.Subtract(start).TotalSeconds < 20)
						Application.RunIteration();
				}

				ApplicationContext.ProgressChanged -= ApplicationContext_ProgressChanged;
				s_splashWindow.Destroy();
				if (minims.IsRunning)
					main = new MainWindow("http://127.0.0.1:9200/org.openiz.core/views/settings/splash.html");
				else return;
			}
			else 
			{

				DcApplicationContext.Current.Started += startHandler;

				while (!started)
					Application.RunIteration();

				ApplicationContext.ProgressChanged -= ApplicationContext_ProgressChanged;
				s_splashWindow.Destroy();
				main = new MainWindow("http://127.0.0.1:9200/org.openiz.core/splash.html");
			}



			if(XamarinApplicationContext.Current.GetService<MiniImsServer>().IsRunning) {
				main.Show ();
			}
		}
	
	}
}
