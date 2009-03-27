namespace sharpGB
{
    partial class MainForm
    {

        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rOMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.headerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.romHexViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.RunStepsButton = new System.Windows.Forms.Button();
            this.VideoBox = new System.Windows.Forms.PictureBox();
            this.RunBox = new System.Windows.Forms.TextBox();
            this.RunButton = new System.Windows.Forms.Button();
            this.InfoBox = new System.Windows.Forms.TextBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.rOMToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(755, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openROMToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openROMToolStripMenuItem
            // 
            this.openROMToolStripMenuItem.Name = "openROMToolStripMenuItem";
            this.openROMToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.openROMToolStripMenuItem.Text = "Open ROM";
            this.openROMToolStripMenuItem.Click += new System.EventHandler(this.openROMToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(130, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // rOMToolStripMenuItem
            // 
            this.rOMToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.headerToolStripMenuItem,
            this.romHexViewToolStripMenuItem});
            this.rOMToolStripMenuItem.Name = "rOMToolStripMenuItem";
            this.rOMToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.rOMToolStripMenuItem.Text = "Debug";
            // 
            // headerToolStripMenuItem
            // 
            this.headerToolStripMenuItem.Name = "headerToolStripMenuItem";
            this.headerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.headerToolStripMenuItem.Text = "ROM Header";
            this.headerToolStripMenuItem.Click += new System.EventHandler(this.headerToolStripMenuItem_Click);
            // 
            // romHexViewToolStripMenuItem
            // 
            this.romHexViewToolStripMenuItem.Name = "romHexViewToolStripMenuItem";
            this.romHexViewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.romHexViewToolStripMenuItem.Text = "ROM Hex View";
            this.romHexViewToolStripMenuItem.Click += new System.EventHandler(this.romHexViewToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Gameboy ROMS|*.gb; *.gbc";
            this.openFileDialog1.InitialDirectory = "/";
            this.openFileDialog1.Title = "Open ROM file";
            // 
            // RunStepsButton
            // 
            this.RunStepsButton.Location = new System.Drawing.Point(343, 28);
            this.RunStepsButton.Name = "RunStepsButton";
            this.RunStepsButton.Size = new System.Drawing.Size(88, 23);
            this.RunStepsButton.TabIndex = 7;
            this.RunStepsButton.Text = "Run Steps:";
            this.RunStepsButton.UseVisualStyleBackColor = true;
            this.RunStepsButton.Click += new System.EventHandler(this.RunStepsButton_Click);
            // 
            // VideoBox
            // 
            this.VideoBox.BackColor = System.Drawing.Color.AliceBlue;
            this.VideoBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.VideoBox.ErrorImage = null;
            this.VideoBox.InitialImage = null;
            this.VideoBox.Location = new System.Drawing.Point(12, 27);
            this.VideoBox.Name = "VideoBox";
            this.VideoBox.Size = new System.Drawing.Size(256, 256);
            this.VideoBox.TabIndex = 9;
            this.VideoBox.TabStop = false;
            // 
            // RunBox
            // 
            this.RunBox.Location = new System.Drawing.Point(437, 30);
            this.RunBox.Name = "RunBox";
            this.RunBox.Size = new System.Drawing.Size(75, 20);
            this.RunBox.TabIndex = 10;
            this.RunBox.Text = "1";
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(274, 29);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(63, 23);
            this.RunButton.TabIndex = 11;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            // 
            // InfoBox
            // 
            this.InfoBox.Location = new System.Drawing.Point(278, 58);
            this.InfoBox.Multiline = true;
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.ReadOnly = true;
            this.InfoBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.InfoBox.Size = new System.Drawing.Size(234, 217);
            this.InfoBox.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 287);
            this.Controls.Add(this.InfoBox);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.RunBox);
            this.Controls.Add(this.VideoBox);
            this.Controls.Add(this.RunStepsButton);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "sharpGB";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openROMToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem rOMToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem headerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem romHexViewToolStripMenuItem;
        private System.Windows.Forms.Button RunStepsButton;
        private System.Windows.Forms.PictureBox VideoBox;
        private System.Windows.Forms.TextBox RunBox;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.TextBox InfoBox;
    }
}

