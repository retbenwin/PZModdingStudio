namespace PZModdingStudio.Forms
{
    partial class FrmFind
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
            this.txtFind = new System.Windows.Forms.TextBox();
            this.chkMatchCase = new System.Windows.Forms.CheckBox();
            this.chkWholeWord = new System.Windows.Forms.CheckBox();
            this.chkRegex = new System.Windows.Forms.CheckBox();
            this.chkWrap = new System.Windows.Forms.CheckBox();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtFind
            // 
            this.txtFind.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtFind.Location = new System.Drawing.Point(0, 0);
            this.txtFind.Name = "txtFind";
            this.txtFind.Size = new System.Drawing.Size(386, 26);
            this.txtFind.TabIndex = 1;
            this.txtFind.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtFind_KeyDown);
            // 
            // chkMatchCase
            // 
            this.chkMatchCase.AutoSize = true;
            this.chkMatchCase.Location = new System.Drawing.Point(12, 45);
            this.chkMatchCase.Name = "chkMatchCase";
            this.chkMatchCase.Size = new System.Drawing.Size(110, 24);
            this.chkMatchCase.TabIndex = 2;
            this.chkMatchCase.Text = "Match case";
            this.chkMatchCase.UseVisualStyleBackColor = true;
            // 
            // chkWholeWord
            // 
            this.chkWholeWord.AutoSize = true;
            this.chkWholeWord.Location = new System.Drawing.Point(12, 75);
            this.chkWholeWord.Name = "chkWholeWord";
            this.chkWholeWord.Size = new System.Drawing.Size(111, 24);
            this.chkWholeWord.TabIndex = 3;
            this.chkWholeWord.Text = "Whole word";
            this.chkWholeWord.UseVisualStyleBackColor = true;
            // 
            // chkRegex
            // 
            this.chkRegex.AutoSize = true;
            this.chkRegex.Location = new System.Drawing.Point(12, 105);
            this.chkRegex.Name = "chkRegex";
            this.chkRegex.Size = new System.Drawing.Size(74, 24);
            this.chkRegex.TabIndex = 4;
            this.chkRegex.Text = "Regex";
            this.chkRegex.UseVisualStyleBackColor = true;
            // 
            // chkWrap
            // 
            this.chkWrap.AutoSize = true;
            this.chkWrap.Location = new System.Drawing.Point(12, 135);
            this.chkWrap.Name = "chkWrap";
            this.chkWrap.Size = new System.Drawing.Size(120, 24);
            this.chkWrap.TabIndex = 5;
            this.chkWrap.Text = "Wrap around";
            this.chkWrap.UseVisualStyleBackColor = true;
            // 
            // btnPrev
            // 
            this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrev.Location = new System.Drawing.Point(12, 178);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(96, 33);
            this.btnPrev.TabIndex = 6;
            this.btnPrev.Text = "Find Prev";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(280, 178);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(96, 33);
            this.btnNext.TabIndex = 7;
            this.btnNext.Text = "Find Next";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // FrmFind
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 220);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.chkWrap);
            this.Controls.Add(this.chkRegex);
            this.Controls.Add(this.chkWholeWord);
            this.Controls.Add(this.chkMatchCase);
            this.Controls.Add(this.txtFind);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MinimumSize = new System.Drawing.Size(259, 259);
            this.Name = "FrmFind";
            this.Text = "Find";
            this.Controls.SetChildIndex(this.txtFind, 0);
            this.Controls.SetChildIndex(this.chkMatchCase, 0);
            this.Controls.SetChildIndex(this.chkWholeWord, 0);
            this.Controls.SetChildIndex(this.chkRegex, 0);
            this.Controls.SetChildIndex(this.chkWrap, 0);
            this.Controls.SetChildIndex(this.btnPrev, 0);
            this.Controls.SetChildIndex(this.btnNext, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFind;
        private System.Windows.Forms.CheckBox chkMatchCase;
        private System.Windows.Forms.CheckBox chkWholeWord;
        private System.Windows.Forms.CheckBox chkRegex;
        private System.Windows.Forms.CheckBox chkWrap;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnNext;
    }
}