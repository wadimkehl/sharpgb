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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.RunStepsButton = new System.Windows.Forms.Button();
            this.VideoBox = new System.Windows.Forms.PictureBox();
            this.RunBox = new System.Windows.Forms.TextBox();
            this.RunButton = new System.Windows.Forms.Button();
            this.InfoBox = new System.Windows.Forms.TextBox();
            this.BreakpointGroupBox = new System.Windows.Forms.GroupBox();
            this.BreakPointAddressTextBox = new System.Windows.Forms.TextBox();
            this.BreakPointListTextBox = new System.Windows.Forms.TextBox();
            this.RemoveBreakPoint = new System.Windows.Forms.Button();
            this.AddBreakPoint = new System.Windows.Forms.Button();
            this.MemoryGroupBox = new System.Windows.Forms.GroupBox();
            this.FollowCodeCheckBox = new System.Windows.Forms.CheckBox();
            this.JumpToTextBox = new System.Windows.Forms.TextBox();
            this.JumpToButton = new System.Windows.Forms.Button();
            this.HexBox = new System.Windows.Forms.RichTextBox();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.VideoBox)).BeginInit();
            this.BreakpointGroupBox.SuspendLayout();
            this.MemoryGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.rOMToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(610, 24);
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
            this.headerToolStripMenuItem});
            this.rOMToolStripMenuItem.Name = "rOMToolStripMenuItem";
            this.rOMToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.rOMToolStripMenuItem.Text = "Debug";
            // 
            // headerToolStripMenuItem
            // 
            this.headerToolStripMenuItem.Name = "headerToolStripMenuItem";
            this.headerToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this.headerToolStripMenuItem.Text = "ROM Header";
            this.headerToolStripMenuItem.Click += new System.EventHandler(this.headerToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Gameboy ROMS|*.gb; *.gbc";
            this.openFileDialog1.InitialDirectory = "/";
            this.openFileDialog1.Title = "Open ROM file";
            // 
            // RunStepsButton
            // 
            this.RunStepsButton.Location = new System.Drawing.Point(278, 27);
            this.RunStepsButton.Name = "RunStepsButton";
            this.RunStepsButton.Size = new System.Drawing.Size(88, 23);
            this.RunStepsButton.TabIndex = 7;
            this.RunStepsButton.Text = "Run Steps:";
            this.RunStepsButton.UseVisualStyleBackColor = true;
            this.RunStepsButton.Click += new System.EventHandler(this.RunStepsButton_Click);
            // 
            // VideoBox
            // 
            this.VideoBox.BackColor = System.Drawing.Color.Black;
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
            this.RunBox.Location = new System.Drawing.Point(372, 29);
            this.RunBox.Name = "RunBox";
            this.RunBox.Size = new System.Drawing.Size(75, 20);
            this.RunBox.TabIndex = 10;
            this.RunBox.Text = "1";
            // 
            // RunButton
            // 
            this.RunButton.Location = new System.Drawing.Point(453, 29);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(63, 23);
            this.RunButton.TabIndex = 11;
            this.RunButton.Text = "Run";
            this.RunButton.UseVisualStyleBackColor = true;
            this.RunButton.Click += new System.EventHandler(this.RunButton_Click);
            // 
            // InfoBox
            // 
            this.InfoBox.BackColor = System.Drawing.Color.CornflowerBlue;
            this.InfoBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InfoBox.Location = new System.Drawing.Point(278, 58);
            this.InfoBox.Multiline = true;
            this.InfoBox.Name = "InfoBox";
            this.InfoBox.ReadOnly = true;
            this.InfoBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.InfoBox.Size = new System.Drawing.Size(234, 225);
            this.InfoBox.TabIndex = 12;
            // 
            // BreakpointGroupBox
            // 
            this.BreakpointGroupBox.Controls.Add(this.BreakPointAddressTextBox);
            this.BreakpointGroupBox.Controls.Add(this.BreakPointListTextBox);
            this.BreakpointGroupBox.Controls.Add(this.RemoveBreakPoint);
            this.BreakpointGroupBox.Controls.Add(this.AddBreakPoint);
            this.BreakpointGroupBox.Location = new System.Drawing.Point(518, 30);
            this.BreakpointGroupBox.Name = "BreakpointGroupBox";
            this.BreakpointGroupBox.Size = new System.Drawing.Size(84, 253);
            this.BreakpointGroupBox.TabIndex = 13;
            this.BreakpointGroupBox.TabStop = false;
            this.BreakpointGroupBox.Text = "Breakpoints";
            // 
            // BreakPointAddressTextBox
            // 
            this.BreakPointAddressTextBox.Location = new System.Drawing.Point(6, 26);
            this.BreakPointAddressTextBox.Name = "BreakPointAddressTextBox";
            this.BreakPointAddressTextBox.Size = new System.Drawing.Size(71, 20);
            this.BreakPointAddressTextBox.TabIndex = 3;
            this.BreakPointAddressTextBox.Text = "FFFF";
            // 
            // BreakPointListTextBox
            // 
            this.BreakPointListTextBox.BackColor = System.Drawing.Color.CornflowerBlue;
            this.BreakPointListTextBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BreakPointListTextBox.Location = new System.Drawing.Point(6, 81);
            this.BreakPointListTextBox.Multiline = true;
            this.BreakPointListTextBox.Name = "BreakPointListTextBox";
            this.BreakPointListTextBox.ReadOnly = true;
            this.BreakPointListTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.BreakPointListTextBox.Size = new System.Drawing.Size(71, 164);
            this.BreakPointListTextBox.TabIndex = 2;
            // 
            // RemoveBreakPoint
            // 
            this.RemoveBreakPoint.Location = new System.Drawing.Point(42, 52);
            this.RemoveBreakPoint.Name = "RemoveBreakPoint";
            this.RemoveBreakPoint.Size = new System.Drawing.Size(35, 25);
            this.RemoveBreakPoint.TabIndex = 1;
            this.RemoveBreakPoint.Text = "-";
            this.RemoveBreakPoint.UseVisualStyleBackColor = true;
            this.RemoveBreakPoint.Click += new System.EventHandler(this.RemoveBreakPoint_Click);
            // 
            // AddBreakPoint
            // 
            this.AddBreakPoint.Location = new System.Drawing.Point(6, 52);
            this.AddBreakPoint.Name = "AddBreakPoint";
            this.AddBreakPoint.Size = new System.Drawing.Size(35, 25);
            this.AddBreakPoint.TabIndex = 0;
            this.AddBreakPoint.Text = "+";
            this.AddBreakPoint.UseVisualStyleBackColor = true;
            this.AddBreakPoint.Click += new System.EventHandler(this.AddBreakPoint_Click);
            // 
            // MemoryGroupBox
            // 
            this.MemoryGroupBox.Controls.Add(this.HexBox);
            this.MemoryGroupBox.Controls.Add(this.JumpToTextBox);
            this.MemoryGroupBox.Controls.Add(this.JumpToButton);
            this.MemoryGroupBox.Controls.Add(this.FollowCodeCheckBox);
            this.MemoryGroupBox.Location = new System.Drawing.Point(12, 289);
            this.MemoryGroupBox.Name = "MemoryGroupBox";
            this.MemoryGroupBox.Size = new System.Drawing.Size(590, 289);
            this.MemoryGroupBox.TabIndex = 16;
            this.MemoryGroupBox.TabStop = false;
            this.MemoryGroupBox.Text = "Memory View";
            // 
            // FollowCodeCheckBox
            // 
            this.FollowCodeCheckBox.AutoSize = true;
            this.FollowCodeCheckBox.Checked = true;
            this.FollowCodeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.FollowCodeCheckBox.Location = new System.Drawing.Point(416, 25);
            this.FollowCodeCheckBox.Name = "FollowCodeCheckBox";
            this.FollowCodeCheckBox.Size = new System.Drawing.Size(148, 17);
            this.FollowCodeCheckBox.TabIndex = 18;
            this.FollowCodeCheckBox.Text = "Follow PC in memory view";
            this.FollowCodeCheckBox.UseVisualStyleBackColor = true;
            // 
            // JumpToTextBox
            // 
            this.JumpToTextBox.Location = new System.Drawing.Point(477, 211);
            this.JumpToTextBox.Name = "JumpToTextBox";
            this.JumpToTextBox.Size = new System.Drawing.Size(38, 20);
            this.JumpToTextBox.TabIndex = 19;
            this.JumpToTextBox.Text = "FFFF";
            // 
            // JumpToButton
            // 
            this.JumpToButton.Location = new System.Drawing.Point(423, 203);
            this.JumpToButton.Name = "JumpToButton";
            this.JumpToButton.Size = new System.Drawing.Size(108, 36);
            this.JumpToButton.TabIndex = 20;
            this.JumpToButton.Text = "Jump To";
            this.JumpToButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.JumpToButton.UseVisualStyleBackColor = true;
            this.JumpToButton.Click += new System.EventHandler(this.JumpToButton_Click);
            // 
            // HexBox
            // 
            this.HexBox.BackColor = System.Drawing.Color.CornflowerBlue;
            this.HexBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.HexBox.DetectUrls = false;
            this.HexBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HexBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.HexBox.Location = new System.Drawing.Point(6, 19);
            this.HexBox.Name = "HexBox";
            this.HexBox.ReadOnly = true;
            this.HexBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.HexBox.Size = new System.Drawing.Size(363, 264);
            this.HexBox.TabIndex = 21;
            this.HexBox.Text = "";
            this.HexBox.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(610, 590);
            this.Controls.Add(this.BreakpointGroupBox);
            this.Controls.Add(this.InfoBox);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.RunBox);
            this.Controls.Add(this.VideoBox);
            this.Controls.Add(this.RunStepsButton);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.MemoryGroupBox);
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
            this.BreakpointGroupBox.ResumeLayout(false);
            this.BreakpointGroupBox.PerformLayout();
            this.MemoryGroupBox.ResumeLayout(false);
            this.MemoryGroupBox.PerformLayout();
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
        private System.Windows.Forms.Button RunStepsButton;
        private System.Windows.Forms.PictureBox VideoBox;
        private System.Windows.Forms.TextBox RunBox;
        private System.Windows.Forms.Button RunButton;
        private System.Windows.Forms.TextBox InfoBox;
        private System.Windows.Forms.GroupBox BreakpointGroupBox;
        private System.Windows.Forms.Button RemoveBreakPoint;
        private System.Windows.Forms.Button AddBreakPoint;
        private System.Windows.Forms.TextBox BreakPointAddressTextBox;
        private System.Windows.Forms.TextBox BreakPointListTextBox;
        private System.Windows.Forms.GroupBox MemoryGroupBox;
        private System.Windows.Forms.CheckBox FollowCodeCheckBox;
        private System.Windows.Forms.TextBox JumpToTextBox;
        private System.Windows.Forms.Button JumpToButton;
        private System.Windows.Forms.RichTextBox HexBox;
    }
}

