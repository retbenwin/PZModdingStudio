using PZModdingStudio.Editor;
using ScintillaNET;
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
        private FrmSolutionExplorer navigationMenu = null;
        private SearchSystem searchSystem = null;

        public DockPanel MainDockPanel
        {
            get { return this.dockPanel; }
        }

        public FrmMainMenu()
        {
            isInitializing = true;
            InitializeComponent();
            this.VisibleLoadingPanelWhenLoading = true;
            isInitializing = false;
        }

        public override void ApplyTranslations()
        {
            base.ApplyTranslations();
            if(searchSystem != null)
                searchSystem.ApplyTranslations();
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
                //this.dockPanel.DockLeftPortion = 135;
                this.dockPanel.DockLeftPortion = 300;
                navigationMenu = new FrmSolutionExplorer(modsManager.SelectedMod);
                navigationMenu.ParentMenuForm = this;
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
            frm.ParentMenuForm = this;
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
                if (navigationMenu != null && !navigationMenu.IsDisposed)
                {
                    navigationMenu.SetMod(modsManager.SelectedMod);
                }
            }
            SetLoading(false);
        }

        private void cboModList_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool changing = false;
            if (modsManager.SelectedMod != (PZTypes.Mod)cboModList.SelectedItem)
            {
                changing = true;
            }
            if (cboModList.SelectedIndex > -1)
            {
                modsManager.SelectedMod = (PZTypes.Mod)cboModList.SelectedItem;
            }
            else
            {
                modsManager.SelectedMod = null;
            }
            if(changing && navigationMenu != null && !navigationMenu.IsDisposed )
            {
                navigationMenu.SetMod(modsManager.SelectedMod);
            }
        }

        private void unloadModToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var election = MessageBox.Show(translator.Get("SureRemoveMod"), translator.Get("SureRemoveModTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(election != DialogResult.Yes)
            {
                return;
            }
            bool unloaded = false;
            foreach (PZTypes.Mod mod in modsManager.Mods)
            {
                if(((ToolStripMenuItem)sender).Text == mod.ToString())
                {
                    unloaded = modsManager.UnloadMod(mod);
                    break;
                }
            }
            if (cboModList.SelectedIndex > -1)
            {
                modsManager.SelectedMod = (PZTypes.Mod)cboModList.SelectedItem;
                if (unloaded && navigationMenu != null && !navigationMenu.IsDisposed)
                {
                    navigationMenu.SetMod(modsManager.SelectedMod);
                }
            }
            else
            {
                modsManager.SelectedMod = null;
                if (unloaded && this.navigationMenu != null && !this.navigationMenu.IsDisposed) {
                    this.navigationMenu.ResetWorkspace();
                }
            }
            FillUnloadMenu();
        }

        private void navigationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FillNavigationMenu();
        }

        private void FrmMainMenu_Load(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                this.Invoke(new Action(() =>
                {
                    PopulateComboMods();
                    FillNavigationMenu();
                }));
            });
            searchSystem = new SearchSystem(this);
            TraslationStatic.ConfigureStaticTranslations(this.translator);
        }

        private void resetZoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var contents = MainDockPanel.Contents;
            foreach (IDockContent content in contents)
            {
                if (content != null && content is FrmCodeEditor)
                {
                    ((FrmCodeEditor)content).ResetZoom();
                }
            }

        }

        private void editorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new FrmCodeEditor();
            frm.Show(this.dockPanel, DockState.Document);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                searchSystem.OpenFindForActiveEditor();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void searchInEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchSystem.OpenFindForActiveEditor();
        }
    }
}
