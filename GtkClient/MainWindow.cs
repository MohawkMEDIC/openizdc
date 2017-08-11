using System;
using Gtk;
using WebKit;
using OpenIZ.Mobile.Core;
using GtkClient;
using OpenIZ.Mobile.Core.Diagnostics;

public partial class MainWindow: Gtk.Window
{

	private Tracer m_tracer = Tracer.GetTracer (typeof(MainWindow));

	private OpenIZWebView m_webView;
	private VBox m_verticalBox;
	private Statusbar m_statusBar;
	private Toolbar m_toolbar;
	private ProgressBar m_progress;

	private Gtk.Action m_actionBack;
	private Gtk.Action m_actionForward;
	private Gtk.Action m_actionReload;


	public MainWindow (String startUrl) : base (Gtk.WindowType.Toplevel)
	{
		this.CreateWidgets ();
		this.Title = "OpenIZ Disconnected Client for Linux";
		this.Build ();
		this.Maximize ();
		this.m_webView.LoadUri (startUrl);
	}

	private void CreateWidgets() {
		this.CreateWebView ();
		this.CreateActions ();
		this.CreateNavBars ();
		this.CreateStatusBar ();

		// Layout
		ScrolledWindow scroll = new ScrolledWindow();
		scroll.Add (this.m_webView);
		this.m_verticalBox = new VBox ();
		this.m_verticalBox.PackStart (this.m_toolbar, false, true, 0);
		this.m_verticalBox.PackStart (scroll);
		this.m_verticalBox.PackEnd (this.m_statusBar, false, true, 0);
		this.Add (this.m_verticalBox);
		this.ShowAll ();
	}

	private void CreateStatusBar() {

		this.m_statusBar = new Statusbar ();
		this.m_statusBar.Push (1, "Idle");
		this.m_progress = new ProgressBar ();
		this.m_statusBar.Add (this.m_progress);
		ApplicationContext.ProgressChanged += (o, e) => {

			Application.Invoke((o1,e1) => {
				this.m_statusBar.Pop (1);
				if (!String.IsNullOrEmpty (e.ProgressText))
					this.m_statusBar.Push (1, e.ProgressText);
				this.m_progress.Fraction = (double)e.Progress;
			});
		};
	}

	private void CreateActions() {
		this.m_actionBack = new Gtk.Action("go-back", "Go Back", null, "gtk-go-back");
		this.m_actionForward = new Gtk.Action("go-forward", "Go Forward", null, "gtk-go-forward");
		this.m_actionReload  = new Gtk.Action("reload", "Reload", null, "gtk-refresh");
		this.m_actionBack.Activated += (o, e) => {
			this.m_webView.GoBack();
		};
		this.m_actionForward.Activated += (o, e) => {
			this.m_webView.GoForward();
		};
		this.m_actionReload.Activated += (o, e) => {
			this.m_webView.Reload();
		};
	}

	private void CreateNavBars() {
		this.m_toolbar = new Toolbar ();
		this.m_toolbar.ToolbarStyle = ToolbarStyle.Icons;
		this.m_toolbar.Orientation = Orientation.Horizontal;
		this.m_toolbar.ShowArrow = true;

		this.m_toolbar.Add (this.m_actionBack.CreateToolItem());
		this.m_toolbar.Add (this.m_actionForward.CreateToolItem());
		this.m_toolbar.Add (this.m_actionReload.CreateToolItem());
	}

	private void CreateWebView() {
		this.m_webView = new OpenIZWebView ();

		this.m_webView.Editable = false;
		this.m_webView.TitleChanged += (o, e) => {
			this.Title = "OpenIZ Disconnected Client for Linux - " + e.Title;
		};
		this.m_webView.Settings = new WebkitSettings("OpenIZ-DC " + ApplicationContext.Current.ExecutionUuid.ToString());
		this.m_webView.Closed += (sender, e) => {

			ApplicationContext.Current.SetProgress ("Shutting down...", 0);
			ApplicationContext.Current.Stop ();
			this.Destroy ();
			Application.Quit();
		};
		this.m_webView.ConsoleMessage += (sender, e) => {
			this.m_tracer.TraceInfo(e.Message);
		};
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		ApplicationContext.Current.SetProgress ("Shutting down...", 0);
		ApplicationContext.Current.Stop ();
		Application.Quit ();
		a.RetVal = true;
	}

	/// <summary>
	/// Releases all resource used by the <see cref="MainWindow"/> object.
	/// </summary>
	/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="MainWindow"/>. The <see cref="Dispose"/>
	/// method leaves the <see cref="MainWindow"/> in an unusable state. After calling <see cref="Dispose"/>, you must
	/// release all references to the <see cref="MainWindow"/> so the garbage collector can reclaim the memory that the
	/// <see cref="MainWindow"/> was occupying.</remarks>

	public override void Dispose ()
	{
		base.Dispose ();
	}
}
