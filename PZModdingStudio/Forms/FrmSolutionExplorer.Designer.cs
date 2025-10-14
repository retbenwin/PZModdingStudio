namespace PZModdingStudio.Forms
{
    partial class FrmSolutionExplorer
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
            this.fileExplorerTree = new FileExplorerTree();
            this.btnReload = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // fileExplorerTree
            // 
            this.fileExplorerTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileExplorerTree.CaretOffset = -12;
            this.fileExplorerTree.CaretSize = 10;
            this.fileExplorerTree.Location = new System.Drawing.Point(3, 32);
            this.fileExplorerTree.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.fileExplorerTree.Name = "fileExplorerTree";
            this.fileExplorerTree.ShowCaret = true;
            this.fileExplorerTree.Size = new System.Drawing.Size(233, 656);
            this.fileExplorerTree.TabIndex = 5;
            // 
            // btnReload
            // 
            this.btnReload.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReload.Location = new System.Drawing.Point(3, 2);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(38, 27);
            this.btnReload.TabIndex = 6;
            this.btnReload.Text = "↻";
            this.btnReload.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip.SetToolTip(this.btnReload, "Reload");
            this.btnReload.UseCompatibleTextRendering = true;
            this.btnReload.UseVisualStyleBackColor = false;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // FrmSolutionExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(235, 692);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.fileExplorerTree);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "FrmSolutionExplorer";
            this.Text = "Mod Explorer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmSolutionExplorer_FormClosed);
            this.Load += new System.EventHandler(this.FrmSolutionExplorer_Load);
            this.Controls.SetChildIndex(this.fileExplorerTree, 0);
            this.Controls.SetChildIndex(this.btnReload, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private FileExplorerTree fileExplorerTree;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.ToolTip toolTip;
    }
}