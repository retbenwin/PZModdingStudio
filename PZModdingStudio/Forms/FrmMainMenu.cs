using Microsoft.VisualBasic;
using PZModdingStudio.Editor;
using PZModdingStudio.Helpers;
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
using static System.Net.Mime.MediaTypeNames;

namespace PZModdingStudio.Forms
{
    public partial class FrmMainMenu : FrmBase
    {

        private bool isInitializing = true;
        private FrmSolutionExplorer navigationMenu = null;
        private SearchSystem searchSystem = null;
        private EditorManager editorManager = null;
        private CommandsManager commandsManager = null;

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
            this.BeginInvoke(new Action(() =>
            {
                PopulateComboMods();
                FillNavigationMenu();
                searchSystem = new SearchSystem(this);
                editorManager = new EditorManager(this);
                commandsManager = new CommandsManager(this);
                commandsManager.OnCommandPressed += CommandPressedHandler;
                TraslationStatic.ConfigureStaticTranslations(this.translator);
            }));
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

        private void CommandPressedHandler(object sender, CommandsManager.CommandPressedArgs e)
        {
            // Postea la acción al loop de mensajes del UI para que se ejecute después
            this.BeginInvoke(new Action(() =>
            {
                switch (e.commandType)
                {
                    case CommandsManager.CommandType.Save:
                        editorManager.SaveFileWithEditor();
                        break;
                    case CommandsManager.CommandType.SaveAll:
                        editorManager.SaveAllFiles();
                        break;
                    case CommandsManager.CommandType.Find:
                        searchSystem.OpenFindForActiveEditor();
                        break;
                }
            }));
        }

        private void searchInEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            searchSystem.OpenFindForActiveEditor();
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileManagement.OpenFile();
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorManager.SaveFileWithEditor();
        }

        private void saveFileAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorManager.SaveFileAsWithEditor();
        }

        private void saveAllFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorManager.SaveAllFiles();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newTextEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editorManager.CreateNewEditor(EditorType.TextEditor);
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            saveAllFilesToolStripMenuItem.Enabled = editorManager.HasChanges();
            saveFileAsToolStripMenuItem.Enabled = editorManager.CurrentEditor?.HasChanges() ?? false;
            saveFileToolStripMenuItem.Enabled = editorManager.CurrentEditor?.HasChanges() ?? false;
        }

        private void FrmMainMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (editorManager.HasChanges())
            {
                var result = MessageBox.Show(translator.Get("UnsavedChangesExit") + Environment.NewLine + translator.Get("AreYouSureExitLoseChanges"), translator.Get("UnsavedChangesExitTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                return;
            }
            if(modsManager.Mods.Count > 0)
            {
                var result = MessageBox.Show((modsManager.Mods.Count > 1 ? translator.Get("ModsLoadedExit") : translator.Get("ModLoadedExit")) + Environment.NewLine + translator.Get("AreYouSureExit"), translator.Get("ModLoadedExitTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                return;
            }

        }

    }
}
