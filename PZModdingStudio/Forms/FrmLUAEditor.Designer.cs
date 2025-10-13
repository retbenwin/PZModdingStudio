namespace PZModdingStudio.Forms
{
    partial class FrmLUAEditor
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
            this.scintilla = new ScintillaNET.Scintilla();
            this.SuspendLayout();
            // 
            // scintilla
            // 
            this.scintilla.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scintilla.Location = new System.Drawing.Point(0, 0);
            this.scintilla.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.scintilla.Name = "scintilla";
            this.scintilla.Size = new System.Drawing.Size(403, 279);
            this.scintilla.TabIndex = 0;
            // 
            // FrmLUAEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(403, 279);
            this.Controls.Add(this.scintilla);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FrmLUAEditor";
            this.Text = "LUA Editor";
            this.Load += new System.EventHandler(this.FrmLUAEditor_Load);
            this.Controls.SetChildIndex(this.scintilla, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla scintilla;
    }
}