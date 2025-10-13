using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PZModdingStudio.Forms
{
    public partial class FrmMainMenu : FrmBase
    {

        private bool isInitializing = true;
        private FrmBase navigationMenu = null;

        public DockPanel MainDockPanel
        {
            get { return this.dockPanel; }
        }

        public FrmMainMenu()
        {
            isInitializing = true;
            InitializeComponent();
            this.VisibleLoadingPanelWhenLoading = true;
            PopulateComboMods();
            FillNavigationMenu();
            isInitializing = false;
        }

        private void PopulateComboMods()
        {
            cboModList.DataSource = modsManager.Mods;
            if (cboModList.Items.Count > 0)
            {
                cboModList.SelectedIndex = 0;
            }
        }

        private void FillNavigationMenu()
        {
            if(navigationMenu == null || navigationMenu.IsDisposed)
            {
                this.dockPanel.DockLeftPortion = 135;
                navigationMenu = new FrmMenuMod();
                navigationMenu.ParentForm = this;
                navigationMenu.Show(this.dockPanel, DockState.DockLeft);
            }
            else
            {
                navigationMenu.Show(this.dockPanel, DockState.DockLeft);
            }
        }

        private void FillUnloadMenu()
        {
            unloadModToolStripMenuItem.DropDownItems.Clear();
            foreach (PZTypes.Mod mod in modsManager.Mods)
            {
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Text = mod.ToString();
                unloadModToolStripMenuItem.DropDownItems.Add(menuItem);
                menuItem.Click += unloadModToolStripMenuItem_Click;
            }
        }

        internal override void LoadingBehaviour(bool isLoading)
        {
            base.LoadingBehaviour(isLoading);
            if(isInitializing) return;
            cboModList.Enabled = !isLoading;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings frm = new FrmSettings();
            frm.ParentForm = this;
            frm.ShowDialog(this);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isInitializing) return;
            SetLoading(true);
            if (modsManager.LoadMod())
            {
                cboModList.SelectedIndex = cboModList.Items.Count - 1;
                modsManager.SelectedMod = (PZTypes.Mod)cboModList.SelectedItem;
                FillUnloadMenu();
            }
            SetLoading(false);
        }

        private void cboModList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cboModList.SelectedIndex > -1)
            {
                modsManager.SelectedMod = (PZTypes.Mod)cboModList.SelectedItem;
            }
            else
            {
                modsManager.SelectedMod = null;
            }
        }

        private void unloadModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var election = MessageBox.Show(translator.Get("SureRemoveMod"), translator.Get("SureRemoveModTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(election != DialogResult.Yes)
            {
                return;
            }
            foreach (PZTypes.Mod mod in modsManager.Mods)
            {
                if(((ToolStripMenuItem)sender).Text == mod.ToString())
                {
                    modsManager.UnloadMod(mod);
                    break;
                }
            }
            if (cboModList.SelectedIndex > -1)
            {
                modsManager.SelectedMod = (PZTypes.Mod)cboModList.SelectedItem;
            }
            else
            {
                modsManager.SelectedMod = null;
            }
            FillUnloadMenu();
        }

        private void navigationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FillNavigationMenu();
        }
    }
}
