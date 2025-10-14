using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PZModdingStudio
{
    public static class TraslationStatic
    {

        public static void ConfigureStaticTranslations(TranslationProvider provider)
        {
            provider.StaticTranslations.Add(ApplyFileExplorerTreeTraslation);
        }

        public static void ApplyFileExplorerTreeTraslation(TranslationProvider provider)
        {
            FileExplorerTree.TextFolder = provider.Get("Folder");
            FileExplorerTree.TextFolder2 = provider.Get("Folder2");
            FileExplorerTree.TextFile = provider.Get("File");
            FileExplorerTree.TextFile2 = provider.Get("File2");
            FileExplorerTree.TextProperties = provider.Get("Properties");
            FileExplorerTree.TextDelete = provider.Get("Delete");
            FileExplorerTree.TextNew = provider.Get("New");
            FileExplorerTree.TextOpen = provider.Get("Open");
            FileExplorerTree.TextOpenFileExplorer = provider.Get("OpenFileExplorer");
            FileExplorerTree.TextCantDeleteRoot = provider.Get("CantDeleteRoot");
            FileExplorerTree.TextWarning = provider.Get("Warning");
            FileExplorerTree.TextError = provider.Get("Error");
            FileExplorerTree.TextCantDeleteError = provider.Get("CantDeleteError");
            FileExplorerTree.TextConfirmDelete = provider.Get("ConfirmDelete");
            FileExplorerTree.TextConfirmDeleteTitle = provider.Get("ConfirmDeleteTitle");
            FileExplorerTree.TextDialogNewFolderInput = provider.Get("DialogNewFolderInput");
            FileExplorerTree.TextNewFolder = provider.Get("NewFolder");
            FileExplorerTree.TextCantCreateFolderError = provider.Get("CantCreateFolderError");
            FileExplorerTree.TextCantCreateFileError = provider.Get("CantCreateFileError");
            FileExplorerTree.TextDialogNewFileInput = provider.Get("DialogNewFileInput");
            FileExplorerTree.TextNewFile = provider.Get("NewFile");
            FileExplorerTree.TextAccept = provider.Get("Accept");
            FileExplorerTree.TextCancel = provider.Get("Cancel");
            FileExplorerTree.TextInput = provider.Get("Input");
            FileExplorerTree.TextCantRenameRoot = provider.Get("CantRenameRoot");
            FileExplorerTree.TextCantRenameError = provider.Get("CantRenameError");
            FileExplorerTree.TextRename = provider.Get("Rename");
            FileExplorerTree.TextFolderAlreadyExists = provider.Get("FolderAlreadyExists");
            FileExplorerTree.TextDialogRenameFolderInput = provider.Get("DialogRenameFolderInput");
            FileExplorerTree.TextDialogRenameFilenput = provider.Get("DialogRenameFilenput");

        }

    }
}
