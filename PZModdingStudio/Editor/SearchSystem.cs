using PZModdingStudio.Forms;
using PZModdingStudio.Lang;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PZModdingStudio.Editor
{
    internal class SearchSystem : ITranslatable
    {

        private static SearchSystem instance;
        private FrmFind findForm;
        private FrmCodeEditor currentEditor;
        private FrmMainMenu frmMainMenu;
        private TranslationProvider translator;

        public SearchSystem(FrmMainMenu frmMainMenu)
        {
            this.frmMainMenu = frmMainMenu;
            frmMainMenu.MainDockPanel.ActiveContentChanged += new System.EventHandler(this.dockPanel_ActiveContentChanged);
            findForm = new FrmFind();
            translator = TranslationProvider.GetInstance();
            ApplyTranslations();
            instance = this;
        }

        public void ApplyTranslations()
        {
            if(translator == null) return;
            SetupTextInFindForm();
        }

        public static SearchSystem GetInstance()
        {
            if (instance == null)
            {
                throw new Exception("searchSystem is null");
            }
            return instance;
        }

        public void SetupTextInFindForm()
        {
            if (translator == null || currentEditor == null) return;
            findForm.Text = $"{translator.Get("Find")} - {currentEditor.Text}";
        }

        public void OpenFindForActiveEditor()
        {
            var ed = frmMainMenu.MainDockPanel.ActiveContent as FrmCodeEditor;
            if (ed == null)
            {
                MessageBox.Show(translator.Get("NoActiveEditor"), "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AttachScintilla(ed.scintillaInstance, ed);
            findForm.ShowCentered(frmMainMenu);
        }

        private void AttachScintilla(Scintilla scintilla, FrmCodeEditor ownerEditor = null)
        {
            if (scintilla == null) return;

            if (currentEditor != null && currentEditor.scintillaInstance == scintilla) return;

            findForm.AttachScintilla(scintilla);

            currentEditor = ownerEditor;
            SetupTextInFindForm();
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            var ed = frmMainMenu.MainDockPanel.ActiveContent as FrmCodeEditor;
            if (ed != null)
            {
                AttachScintilla(ed.scintillaInstance, ed);
            }
        }

    }
}
