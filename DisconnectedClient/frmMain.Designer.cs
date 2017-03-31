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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lnkMain = new System.Windows.Forms.LinkLabel();
            this.lblProgress = new System.Windows.Forms.Label();
            this.pgMain = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::DisconnectedClient.Properties.Resources.logo_lg;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(120, 122);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(126, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(500, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "OpenImmunize Disconnected Client";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(128, 41);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(51, 20);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "label2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(158, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Open User Interface:";
            // 
            // lnkMain
            // 
            this.lnkMain.AutoSize = true;
            this.lnkMain.Location = new System.Drawing.Point(292, 72);
            this.lnkMain.Name = "lnkMain";
            this.lnkMain.Size = new System.Drawing.Size(80, 20);
            this.lnkMain.TabIndex = 4;
            this.lnkMain.TabStop = true;
            this.lnkMain.Text = "linkLabel1";
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 125);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(102, 20);
            this.lblProgress.TabIndex = 5;
            this.lblProgress.Text = "Starting Up...";
            // 
            // pgMain
            // 
            this.pgMain.Location = new System.Drawing.Point(16, 148);
            this.pgMain.Name = "pgMain";
            this.pgMain.Size = new System.Drawing.Size(640, 23);
            this.pgMain.TabIndex = 6;
            // 
            // frmDisconnectedClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(668, 177);
            this.Controls.Add(this.pgMain);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.lnkMain);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "frmDisconnectedClient";
            this.Text = "OpenIZ Disconnected Client";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel lnkMain;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar pgMain;
    }
}

