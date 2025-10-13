// TranslationProvider.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;

namespace PZModdingStudio
{
    public class TranslationProvider
    {
        private static TranslationProvider _instance;


        // Estructura: language -> (key -> value)
        private readonly Dictionary<string, Dictionary<string, string>> languagesStore
            = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);

        private readonly string langsFolder = Path.Combine(Application.StartupPath, "Lang");

        private readonly char scapeComma = '&';

        public string CurrentLanguage { get; private set; }
        public string FallbackLanguage { get; set; } = "en";

        // Devuelve los idiomas disponibles
        public IEnumerable<string> GetAvailableLanguages() => languagesStore.Keys;

        public TranslationProvider(string lang)
        {
            this.LoadLanguageFolder(this.langsFolder);
            if (!this.SetLanguage(lang)) this.SetLanguage(this.FallbackLanguage);
        }

        // Carga todos los archivos CSV de la carpeta lang (ej: lang\en.csv, lang\es.csv)
        // espera archivos con encabezado Key,Value y encoding UTF-8
        public void LoadLanguageFolder(string folderPath, char delimiter = ',')
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show($"Language folder not found: {folderPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var files = Directory.GetFiles(folderPath, "*.csv");
            foreach (var f in files) LoadLanguageFile(f, delimiter);
        }

        // Carga un archivo CSV de un solo idioma; el nombre del archivo (sin extensión) será el código de idioma.
        public void LoadLanguageFile(string path, char delimiter = ',')
        {
            if (!File.Exists(path)) return;
            string lang = Path.GetFileNameWithoutExtension(path);
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var parser = new TextFieldParser(path))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(delimiter.ToString());
                parser.HasFieldsEnclosedInQuotes = true;
                parser.TrimWhiteSpace = true;

                // header
                if (!parser.EndOfData)
                {
                    var headers = parser.ReadFields();
                    // continua leyendo filas
                    while (!parser.EndOfData)
                    {
                        var fields = parser.ReadFields();
                        if (fields == null || fields.Length == 0) continue;
                        string key = fields[0]?.Trim();
                        if (string.IsNullOrEmpty(key)) continue;
                        string val = fields.Length > 1 ? fields[1] ?? string.Empty : string.Empty;
                        dict[key] = val; // sobrescribe si existe
                    }
                }
            }

            languagesStore[lang] = dict;
        }

        // Cambia idioma actual (si no existe, no hace nada)
        public bool SetLanguage(string lang)
        {
            if (string.IsNullOrEmpty(lang)) return false;
            if (!languagesStore.ContainsKey(lang))
            {
                MessageBox.Show($"Language not found: {lang}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            CurrentLanguage = lang;
            return true;
        }

        // Obtener traducción con fallback
        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key)) return string.Empty;

            if (!string.IsNullOrEmpty(CurrentLanguage) && languagesStore.TryGetValue(CurrentLanguage, out var cur) &&
                cur.TryGetValue(key, out var v) && !string.IsNullOrEmpty(v))
                return v;

            if (!string.IsNullOrEmpty(FallbackLanguage) && languagesStore.TryGetValue(FallbackLanguage, out var fb) &&
                fb.TryGetValue(key, out var v2) && !string.IsNullOrEmpty(v2))
                return v2;

            // no encontrado: devuelve en inglés si es posible
            if (!string.Equals(CurrentLanguage, "en", StringComparison.OrdinalIgnoreCase) &&
                languagesStore.TryGetValue("en", out var en) &&
                en.TryGetValue(key, out var v3) && !string.IsNullOrEmpty(v3))
                return v3;

            // último recurso: devolver la key (o string.Empty si prefieres)
            return key;
        }

        // Aplica traducciones a un Form (incluye controles, menús y toolstrip items)
        public void ApplyTranslations(Form form)
        {
            if (form == null) return;

            // Título del formulario
            string formTitleKey = $"{form.Name}.$this.Text";
            string formTitle = Get(formTitleKey);
            if (!string.IsNullOrEmpty(formTitle) && formTitle != formTitleKey) form.Text = formTitle;

            // Recorrer controles
            void ApplyToControl(Control c)
            {
                if (c == null) return;
                // Text property
                string keyText = $"{form.Name}.{c.Name}.Text";
                var txt = Get(keyText);
                if (!string.IsNullOrEmpty(txt) && txt != keyText) c.Text = txt;

                foreach (Control child in c.Controls) ApplyToControl(child);
            }

            foreach (Control c in form.Controls) ApplyToControl(c);

            // Menus (MenuStrip)
            foreach (var ms in FindControlsOfType<MenuStrip>(form))
                foreach (ToolStripItem it in ms.Items) ApplyToToolStripItemRecursive(form, it);

            // ToolStrips, ContextMenuStrip en controles
            foreach (var strip in FindControlsOfType<ToolStrip>(form))
                foreach (ToolStripItem it in strip.Items) ApplyToToolStripItemRecursive(form, it);

            foreach (Control c in GetAllControls(form))
            {
                if (c.ContextMenuStrip != null)
                {
                    foreach (ToolStripItem it in c.ContextMenuStrip.Items) ApplyToToolStripItemRecursive(form, it);
                }
            }
        }

        private void ApplyToToolStripItemRecursive(Form form, ToolStripItem item)
        {
            if (item == null) return;
            string key = $"{form.Name}.{item.Name}.Text";
            var txt = Get(key);
            if (!string.IsNullOrEmpty(txt) && txt != key) item.Text = txt;

            if (item is ToolStripMenuItem menuItem && menuItem.HasDropDownItems)
            {
                foreach (ToolStripItem child in menuItem.DropDownItems)
                    ApplyToToolStripItemRecursive(form, child);
            }
        }

        // Helpers
        private IEnumerable<T> FindControlsOfType<T>(Control parent) where T : Control
        {
            var list = new List<T>();
            foreach (Control c in GetAllControls(parent))
                if (c is T t) list.Add(t);
            return list;
        }

        private IEnumerable<Control> GetAllControls(Control parent)
        {
            var stack = new Stack<Control>();
            stack.Push(parent);
            while (stack.Count > 0)
            {
                var c = stack.Pop();
                yield return c;
                foreach (Control child in c.Controls) stack.Push(child);
            }
        }

        public static TranslationProvider GetInstance()
        {
            if (_instance == null)
            {
                var settings = Config.Settings.Get();
                _instance = new TranslationProvider(settings.Language);
            }
            return _instance;
        }

    }
}
