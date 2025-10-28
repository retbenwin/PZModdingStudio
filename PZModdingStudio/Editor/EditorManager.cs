using PZModdingStudio.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PZModdingStudio.Editor
{
    public class EditorManager
    {

        private FrmMainMenu frmMainMenu;
        private List<IEditor> openEditors;
        private static EditorManager instance;
        private IEditor currentEditor;

        public IReadOnlyList<IEditor> OpenEditors
        {
            get { return openEditors.AsReadOnly(); }
        }

        public IEditor CurrentEditor
        {
            get { return currentEditor; }
        }

        public EditorManager(FrmMainMenu frmMainMenu)
        {
            this.frmMainMenu = frmMainMenu;
            openEditors = new List<IEditor>();
            instance = this;
            currentEditor = null;
            frmMainMenu.MainDockPanel.ActiveContentChanged += new System.EventHandler(this.dockPanel_ActiveContentChanged);
        }

        public static EditorManager GetInstance()
        {
            if(instance == null)
            {
                throw new Exception("EditorManager instance is not initialized.");
            }
            return instance;
        }

        public IEditor OpenFileWithEditor(string filePath, EditorType type)
        {
            IEditor frmCodeEditor = FileOpened(filePath);
            if(frmCodeEditor != null)
            {
                frmCodeEditor.Use();
            }
            else
            {
                frmCodeEditor = OpenEditor(filePath, type);
            }
            return frmCodeEditor;
        }


        public bool IsFileOpened(string filePath)
        {
            foreach (IEditor editor in openEditors)
            {
                if (editor.CurrentFile == filePath)
                {
                    return true;
                }
            }
            return false;
        }

        public IEditor FileOpened(string filePath)
        {
            foreach(IEditor editor in openEditors)
            {
                if(editor.CurrentFile == filePath)
                {
                    return editor;
                }
            }
            return null;
        }

        public IEditor OpenEditor(string filePath, EditorType type)
        {
            IEditor frmCodeEditor = CreateNewEditor(type);
            frmCodeEditor.OpenFile(filePath);
            return frmCodeEditor;
        }

        public IEditor CreateNewEditor()
        {
            return CreateNewEditor(EditorType.TextEditor);
        }

        public IEditor CreateNewEditor(EditorType type)
        {
            IEditor editor;
            switch(type)
            {
                case EditorType.TextEditor:
                    editor = CreateNewTextEditor();
                    break;
                default:
                    editor = CreateNewTextEditor();
                    break;
            }

            openEditors.Add(editor);
            // Handle editor closing to remove from list
            editor.FormClosed += frmCodeEditor_FormClosed;
            editor.UpdatedTitle += frmEditor_UpdatedTitle;
            return editor;
        }

        private IEditor CreateNewTextEditor()
        {
            FrmCodeEditor frmCodeEditor = new FrmCodeEditor();
            frmCodeEditor.Show(frmMainMenu.MainDockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            return frmCodeEditor;
        }

        public void SaveFileWithEditor()
        {
            if(currentEditor != null && currentEditor.HasChanges())
            {
                currentEditor.SaveFile();
            }
        }

        public void SaveFileAsWithEditor()
        {
            if (currentEditor != null && currentEditor.HasChanges())
            {
                currentEditor.SaveFileAs();
            }
        }

        public bool SaveAllFiles()
        {
            foreach(IEditor editor in openEditors)
            {
                if (editor.HasChanges())
                {
                    if (!editor.SaveFile())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool HasChanges()
        {
            foreach(IEditor editor in openEditors)
            {
                if(editor.HasChanges())
                {
                    return true;
                }
            }
            return false;
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            object ed = frmMainMenu.MainDockPanel.ActiveContent;
            if (ed != null && ed is IEditor)
            {
                currentEditor = (IEditor)ed;
                return;
            }
            if(openEditors.Count == 0)
            {
                currentEditor = null;
            }
        }

        private void frmCodeEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            IEditor editor = (IEditor)sender;
            openEditors.Remove(editor);
            editor.FormClosed -= frmCodeEditor_FormClosed;
        }

        private void frmEditor_UpdatedTitle(object sender, EventArgs e)
        {
            SearchSystem.GetInstance().SetupTextInFindForm();
        }


    }
}
