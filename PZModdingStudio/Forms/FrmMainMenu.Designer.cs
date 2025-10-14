namespace PZModdingStudio.Forms
{
    partial class FrmMainMenu
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unloadModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.navigationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cboModList = new System.Windows.Forms.ComboBox();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.vS2015LightTheme = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();
            this.msMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // msMain
            // 
            this.msMain.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(784, 29);
            this.msMain.TabIndex = 2;
            this.msMain.Text = "msMain";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.unloadModToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(46, 25);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.loadToolStripMenuItem.Text = "Load Mod";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // unloadModToolStripMenuItem
            // 
            this.unloadModToolStripMenuItem.Name = "unloadModToolStripMenuItem";
            this.unloadModToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.unloadModToolStripMenuItem.Text = "Unload Mod";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(166, 26);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.navigationToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(56, 25);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // navigationToolStripMenuItem
            // 
            this.navigationToolStripMenuItem.Name = "navigationToolStripMenuItem";
            this.navigationToolStripMenuItem.Size = new System.Drawing.Size(156, 26);
            this.navigationToolStripMenuItem.Text = "Navigation";
            this.navigationToolStripMenuItem.Click += new System.EventHandler(this.navigationToolStripMenuItem_Click);
            // 
            // cboModList
            // 
            this.cboModList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboModList.FormattingEnabled = true;
            this.cboModList.Location = new System.Drawing.Point(13, 39);
            this.cboModList.Name = "cboModList";
            this.cboModList.Size = new System.Drawing.Size(390, 28);
            this.cboModList.TabIndex = 3;
            this.cboModList.SelectedIndexChanged += new System.EventHandler(this.cboModList_SelectedIndexChanged);
            // 
            // dockPanel
            // 
            this.dockPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(238)))), ((int)(((byte)(242)))));
            this.dockPanel.Location = new System.Drawing.Point(13, 76);
            this.dockPanel.Name = "dockPanel";
            this.dockPanel.Padding = new System.Windows.Forms.Padding(6);
            this.dockPanel.ShowAutoHideContentOnHover = false;
            this.dockPanel.Size = new System.Drawing.Size(759, 473);
            this.dockPanel.TabIndex = 0;
            this.dockPanel.Theme = this.vS2015LightTheme;
            // 
            // FrmMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.cboModList);
            this.Controls.Add(this.msMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MainMenuStrip = this.msMain;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "FrmMainMenu";
            this.Text = "Project Zomboid Modding Studio";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FrmMainMenu_Load);
            this.Controls.SetChildIndex(this.msMain, 0);
            this.Controls.SetChildIndex(this.cboModList, 0);
            this.Controls.SetChildIndex(this.dockPanel, 0);
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ComboBox cboModList;
        private System.Windows.Forms.ToolStripMenuItem unloadModToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private WeifenLuo.WinFormsUI.Docking.VS2015LightTheme vS2015LightTheme;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem navigationToolStripMenuItem;
    }
}

