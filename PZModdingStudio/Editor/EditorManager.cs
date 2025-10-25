using PZModdingStudio.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZModdingStudio.Editor
{
    public class EditorManager
    {

        FrmMainMenu frmMenu;
        List<FrmCodeEditor> openEditors;
        static EditorManager instance;

        public EditorManager(FrmMainMenu frmMenu)
        {
            this.frmMenu = frmMenu;
            openEditors = new List<FrmCodeEditor>();
            instance = this;
        }

        public static EditorManager GetInstance()
        {
            if(instance == null)
            {
                throw new Exception("EditorManager instance is not initialized.");
            }
            return instance;
        }

        public FrmCodeEditor OpenFileWithEditor(string filePath)
        {
            FrmCodeEditor frmCodeEditor = FileOpened(filePath);
            if(frmCodeEditor != null)
            {
                frmCodeEditor.Focus();
                frmCodeEditor.Activate();
            }
            else
            {
                frmCodeEditor = OpenEditor(filePath);
            }
            return frmCodeEditor;
        }

        public bool IsFileOpened(string filePath)
        {
            foreach (FrmCodeEditor editor in openEditors)
            {
                if (editor.CurrentFile == filePath)
                {
                    return true;
                }
            }
            return false;
        }

        public FrmCodeEditor FileOpened(string filePath)
        {
            foreach(FrmCodeEditor editor in openEditors)
            {
                if(editor.CurrentFile == filePath)
                {
                    return editor;
                }
            }
            return null;
        }

        public FrmCodeEditor OpenEditor(string filePath)
        {
            FrmCodeEditor frmCodeEditor = CreateNewEditor();
            frmCodeEditor.OpenFile(filePath);
            return frmCodeEditor;
        }

        public FrmCodeEditor CreateNewEditor()
        {
            FrmCodeEditor frmCodeEditor = new FrmCodeEditor();
            frmCodeEditor.Show(frmMenu.MainDockPanel, WeifenLuo.WinFormsUI.Docking.DockState.Document);
            openEditors.Add(frmCodeEditor);
            // Handle editor closing to remove from list
            frmCodeEditor.FormClosed += (s, e) =>
            {
                openEditors.Remove(frmCodeEditor);
            };
            return frmCodeEditor;
        }

    }
}
