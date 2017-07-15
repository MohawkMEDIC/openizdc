using System;
using Gtk;
using WebKit;
using OpenIZ.Mobile.Core;
using GtkClient;

public partial class MainWindow: Gtk.Window
{

	private WebView m_webView;
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
		this.m_webView = new WebView ();

		this.m_webView.Editable = false;
		this.m_webView.TitleChanged += (o, e) => {
			this.Title = "OpenIZ Disconnected Client for Linux - " + e.Title;
		};
		this.m_webView.Settings = new WebkitSettings("OpenIZ-DC " + ApplicationContext.Current.ExecutionUuid.ToString());
	}

	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
}
