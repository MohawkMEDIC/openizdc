namespace DisconnectedClient
{
    partial class frmDisconnectedClient
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDisconnectedClient));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pgMain = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
            this.btnZoomWidth = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoom150 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoom125 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoom100 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoom75 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoom50 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoom25 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnsTools = new System.Windows.Forms.MenuStrip();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showDebugToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tspMain = new System.Windows.Forms.ToolStrip();
            this.btnBack = new System.Windows.Forms.ToolStripButton();
            this.btnForward = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.statusStrip1.SuspendLayout();
            this.mnsTools.SuspendLayout();
            this.tspMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.pgMain,
            this.toolStripSplitButton1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 387);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(831, 30);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(674, 25);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Idle";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pgMain
            // 
            this.pgMain.Name = "pgMain";
            this.pgMain.Size = new System.Drawing.Size(100, 24);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomWidth,
            this.btnZoom150,
            this.btnZoom125,
            this.btnZoom100,
            this.btnZoom75,
            this.btnZoom50,
            this.btnZoom25});
            this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(40, 28);
            this.toolStripSplitButton1.Text = "toolStripSplitButton1";
            // 
            // btnZoomWidth
            // 
            this.btnZoomWidth.Name = "btnZoomWidth";
            this.btnZoomWidth.Size = new System.Drawing.Size(106, 22);
            this.btnZoomWidth.Tag = "0";
            this.btnZoomWidth.Text = "Width";
            this.btnZoomWidth.Click += new System.EventHandler(this.btnZoomWidth_Click);
            // 
            // btnZoom150
            // 
            this.btnZoom150.Name = "btnZoom150";
            this.btnZoom150.Size = new System.Drawing.Size(106, 22);
            this.btnZoom150.Tag = "2";
            this.btnZoom150.Text = "200%";
            this.btnZoom150.Click += new System.EventHandler(this.btnZoomWidth_Click);
            // 
            // btnZoom125
            // 
            this.btnZoom125.Name = "btnZoom125";
            this.btnZoom125.Size = new System.Drawing.Size(106, 22);
            this.btnZoom125.Tag = "1.5";
            this.btnZoom125.Text = "150%";
            this.btnZoom125.Click += new System.EventHandler(this.btnZoomWidth_Click);
            // 
            // btnZoom100
            // 
            this.btnZoom100.Name = "btnZoom100";
            this.btnZoom100.Size = new System.Drawing.Size(106, 22);
            this.btnZoom100.Tag = "1";
            this.btnZoom100.Text = "100%";
            this.btnZoom100.Click += new System.EventHandler(this.btnZoomWidth_Click);
            // 
            // btnZoom75
            // 
            this.btnZoom75.Name = "btnZoom75";
            this.btnZoom75.Size = new System.Drawing.Size(106, 22);
            this.btnZoom75.Tag = "0.75";
            this.btnZoom75.Text = "75%";
            this.btnZoom75.Click += new System.EventHandler(this.btnZoomWidth_Click);
            // 
            // btnZoom50
            // 
            this.btnZoom50.Name = "btnZoom50";
            this.btnZoom50.Size = new System.Drawing.Size(106, 22);
            this.btnZoom50.Tag = "0.5";
            this.btnZoom50.Text = "50%";
            this.btnZoom50.Click += new System.EventHandler(this.btnZoomWidth_Click);
            // 
            // btnZoom25
            // 
            this.btnZoom25.Name = "btnZoom25";
            this.btnZoom25.Size = new System.Drawing.Size(106, 22);
            this.btnZoom25.Tag = "0.25";
            this.btnZoom25.Text = "25%";
            this.btnZoom25.Click += new System.EventHandler(this.btnZoomWidth_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // mnsTools
            // 
            this.mnsTools.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mnsTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem});
            this.mnsTools.Location = new System.Drawing.Point(0, 0);
            this.mnsTools.Name = "mnsTools";
            this.mnsTools.Size = new System.Drawing.Size(831, 24);
            this.mnsTools.TabIndex = 3;
            this.mnsTools.Text = "menuStrip1";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showDebugToolsToolStripMenuItem});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.debugToolStripMenuItem.Text = "&Debug";
            // 
            // showDebugToolsToolStripMenuItem
            // 
            this.showDebugToolsToolStripMenuItem.Name = "showDebugToolsToolStripMenuItem";
            this.showDebugToolsToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.showDebugToolsToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.showDebugToolsToolStripMenuItem.Text = "Show Debug Tools";
            this.showDebugToolsToolStripMenuItem.Click += new System.EventHandler(this.showDebugToolsToolStripMenuItem_Click);
            // 
            // tspMain
            // 
            this.tspMain.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.tspMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnBack,
            this.btnForward,
            this.toolStripSeparator1,
            this.btnRefresh});
            this.tspMain.Location = new System.Drawing.Point(0, 24);
            this.tspMain.Name = "tspMain";
            this.tspMain.Size = new System.Drawing.Size(831, 39);
            this.tspMain.Stretch = true;
            this.tspMain.TabIndex = 5;
            this.tspMain.Text = "toolStrip1";
            // 
            // btnBack
            // 
            this.btnBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnBack.Image = ((System.Drawing.Image)(resources.GetObject("btnBack.Image")));
            this.btnBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(36, 36);
            this.btnBack.Text = "Back";
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // btnForward
            // 
            this.btnForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnForward.Image = ((System.Drawing.Image)(resources.GetObject("btnForward.Image")));
            this.btnForward.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(36, 36);
            this.btnForward.Text = "Forward";
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(36, 36);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 63);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(831, 324);
            this.pnlMain.TabIndex = 6;
            // 
            // frmDisconnectedClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(831, 417);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.tspMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.mnsTools);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.mnsTools;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "frmDisconnectedClient";
            this.Text = "OpenIZ Disconnected Client";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDisconnectedClient_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.mnsTools.ResumeLayout(false);
            this.mnsTools.PerformLayout();
            this.tspMain.ResumeLayout(false);
            this.tspMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripProgressBar pgMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.MenuStrip mnsTools;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showDebugToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSplitButton toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem btnZoomWidth;
        private System.Windows.Forms.ToolStripMenuItem btnZoom150;
        private System.Windows.Forms.ToolStripMenuItem btnZoom125;
        private System.Windows.Forms.ToolStripMenuItem btnZoom100;
        private System.Windows.Forms.ToolStripMenuItem btnZoom75;
        private System.Windows.Forms.ToolStripMenuItem btnZoom50;
        private System.Windows.Forms.ToolStripMenuItem btnZoom25;
        private System.Windows.Forms.ToolStrip tspMain;
        private System.Windows.Forms.ToolStripButton btnBack;
        private System.Windows.Forms.ToolStripButton btnForward;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.Panel pnlMain;
    }
}

