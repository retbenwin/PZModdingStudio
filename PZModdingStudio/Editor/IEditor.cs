using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PZModdingStudio.Editor
{

    public enum EditorType
    {
        [Description("TextEditor")]
        TextEditor,

        [Description("ModInfoEditor")]
        ModInfoEditor,

        [Description("ImageEditor")]
        ImageEditor,

        [Description("ImageViewer")]
        ImageViewer,

        [Description("AudioEditor")]
        AudioEditor,

        [Description("AudioPlayer")]
        AudioPlayer,

        [Description("VideoEditor")]
        VideoEditor,

        [Description("VideoPlayer")]
        VideoPlayer,

        [Description("Models3DEditor")]
        Models3DEditor,

        [Description("Models3DViewer")]
        Models3DViewer,
    }

    public interface IEditor
    {

        EditorType Type { get; }

        string CurrentFile { get; }

        bool SaveFile();

        bool SaveFileAs();

        void OpenFile(string filePath);

        void Use();

        bool HasChanges();

        event FormClosedEventHandler FormClosed;

        event EventHandler UpdatedTitle;

    }
}
