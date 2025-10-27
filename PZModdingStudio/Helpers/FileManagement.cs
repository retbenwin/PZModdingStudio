using PZModdingStudio.Editor;
using PZModdingStudio.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PZModdingStudio.Helpers.FileManagement;

namespace PZModdingStudio.Helpers
{
    public static class FileManagement
    {

        /// <summary>
        /// Contenedor para la información de un tipo de archivo: nombre lógico y editores soportados.
        /// </summary>
        private class FileTypeInfo
        {
            public string TypeName { get; set; }
            public List<EditorType> SupportedEditors { get; set; }

            public FileTypeInfo(string typeName, params EditorType[] editors)
            {
                TypeName = typeName ?? string.Empty;
                SupportedEditors = editors?.ToList() ?? new List<EditorType>();
            }
        }

        // Diccionario de extensiones -> información del tipo de archivo
        // Rellena o modifica las entradas según necesites.
        private static readonly Dictionary<string, FileTypeInfo> supportedExtensions = new Dictionary<string, FileTypeInfo>(StringComparer.OrdinalIgnoreCase)
        {
            { ".info", new FileTypeInfo("InfoFile", EditorType.ModInfoEditor, EditorType.TextEditor ) },
            { ".txt",  new FileTypeInfo("TextFile", EditorType.TextEditor) },
            { ".lua",  new FileTypeInfo("LuaScriptFile", EditorType.TextEditor) },
            { ".xml",  new FileTypeInfo("XMLFile", EditorType.TextEditor) },
            { ".json", new FileTypeInfo("JSONFile", EditorType.TextEditor) },
        };

        public static bool IsSupportedFileExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
                return false;

            return supportedExtensions.ContainsKey(extension);
        }

        public static string GetFileTypeByExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "unknown";

            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
                return "unknown";

            if (supportedExtensions.TryGetValue(extension, out FileTypeInfo fileInfo))
            {
                return fileInfo.TypeName;
            }

            return "unknown";
        }

        public static string GetFileNameByExtension(string fileName)
        {
            string type = GetFileTypeByExtension(fileName);
            return TranslationProvider.GetInstance().Get(type);
        }

        /// <summary>
        /// Devuelve la lista (solo lectura) de editores soportados para la extensión del archivo proporcionado.
        /// Si la extensión no está soportada devuelve una colección vacía.
        /// </summary>
        public static IReadOnlyList<EditorType> GetSupportedEditorsByExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Array.Empty<EditorType>();

            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (string.IsNullOrEmpty(extension))
                return Array.Empty<EditorType>();

            if (supportedExtensions.TryGetValue(extension, out FileTypeInfo fileInfo))
            {
                return fileInfo.SupportedEditors.AsReadOnly();
            }

            return Array.Empty<EditorType>();
        }

        /// <summary>
        /// Indica si la extensión soporta un editor concreto.
        /// </summary>
        public static bool HasEditorForExtensionFileName(string fileName, EditorType editor)
        {
            var editors = GetSupportedEditorsByExtension(fileName);
            return editors.Contains(editor);
        }

        /// <summary>
        /// Indica si la extensión soporta un editor concreto.
        /// </summary>
        public static bool HasEditorForExtensionFilePath(string filePath, EditorType editor)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return false;
            string fileName = Path.GetFileName(filePath);
            var editors = GetSupportedEditorsByExtension(fileName);
            return editors.Contains(editor);
        }

        public static void OpenFile()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = $"{TranslationProvider.GetInstance().Get("LoadModDialogFilterAllFIles")} (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (IsSupportedFileExtension(dlg.FileName) || !Path.HasExtension(dlg.FileName))
                    {
                        OpenFile(dlg.FileName);
                    }
                    else
                    {
                        MessageBox.Show(TranslationProvider.GetInstance().Get("UnsupportedFileType"), TranslationProvider.GetInstance().Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        public static void OpenFile(string filePath)
        {
            // Usamos directamente el path: Path.GetExtension funciona tanto con nombres como con paths completos
            if (File.Exists(filePath) && !Path.HasExtension(filePath))
            {
                //Archivo sin extensión, abrir siempre con el editor de texto.
                OpenFileWithEditor(filePath);
                return;
            }

            if (IsSupportedFileExtension(filePath))
            {
                OpenFileWithFirstEditor(filePath);
            }
            else
            {
                OpenFileWithDefaultProgram(filePath);
            }
        }

        public static void OpenFileWithDefaultProgram(string filePath)
        {
            System.Diagnostics.Process.Start(filePath);
        }

        public static void OpenFileWithEditor(string filePath)
        {
            OpenFileWithEditor(filePath, EditorType.TextEditor);
        }

        public static void OpenFileWithEditor(string filePath, EditorType editorType)
        {
            if(HasEditorForExtensionFilePath(filePath, editorType))
            {
                EditorManager.GetInstance().OpenFileWithEditor(filePath, editorType);
                return;
            }
            OpenFileWithDefaultProgram(filePath);
        }

        public static void OpenFileWithFirstEditor(string filePath)
        {
            IReadOnlyList<EditorType> editorTypes = GetSupportedEditorsByExtension(filePath);
            if (editorTypes.Count > 0)
            {
                EditorManager.GetInstance().OpenFileWithEditor(filePath, editorTypes[0]);
                return;
            }
            OpenFileWithDefaultProgram(filePath);
        }

        public static string GetEditorTypeDescription(EditorType value)
        {
            FieldInfo field = typeof(EditorType).GetField(value.ToString());

            DescriptionAttribute attribute =
                (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            return attribute?.Description ?? value.ToString();
        }


    }
}
