using PZModdingStudio.PZTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FileExplorerTree;

namespace PZModdingStudio.Forms
{
    public partial class FrmSolutionExplorer : FrmBase
    {

        private Mod mod;

        public FrmSolutionExplorer(Mod mod)
        {
            this.mod = mod;
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED
                return cp;
            }
        }

        public void SetMod(Mod mod)
        {
            if(this.mod != null && this.mod != mod)
            {
                string status = fileExplorerTree.GetStateString();
                this.mod.SetWorkspaceStatus(status);
            }
            this.mod = mod;
            if(mod != null && !string.IsNullOrEmpty(mod.GetWorkspacePath()) && System.IO.Directory.Exists(mod.GetWorkspacePath()))
            {
                string status = mod.GetWorkspaceStatus();
                if (status != null)
                {
                    fileExplorerTree.SetStateFromString(status);
                }
                else
                {
                    fileExplorerTree.SetRoot(mod.GetWorkspacePath(), false, 2);
                }
                this.AutoScrollPosition = new Point(0, 0);
            }
        }

        private void FrmSolutionExplorer_Load(object sender, EventArgs e)
        {
            if (mod != null)
            {
                SetMod(mod);
            }
        }

        private void FrmSolutionExplorer_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mod != null)
            {
                string status = fileExplorerTree.GetStateString();
                mod.SetWorkspaceStatus(status);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            if (fileExplorerTree.GetCurrentWorkspaceState().RootPath != null)
            {
                fileExplorerTree.RefreshWorkspace();
            }
        }

        public void ResetWorkspace()
        {
            fileExplorerTree.ResetWorkspace();
        }

        public override void ApplyTranslations()
        {
            base.ApplyTranslations();
            toolTip.SetToolTip(btnReload, translator.Get("Reload") );
        }

    }



}
