using PZModdingStudio.Editor;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static ScintillaNET.Style;

namespace PZModdingStudio.Forms
{
    public partial class FrmCodeEditor : FrmBase, IEditor
    {
        private string currentFile = null;

        // Lenguajes registrados
        private ILanguageDefinition currentLanguage = null;
        private readonly Dictionary<string, ILanguageDefinition> registeredLanguages =
            new Dictionary<string, ILanguageDefinition>(StringComparer.OrdinalIgnoreCase);

        public  Scintilla scintillaInstance { get { return scintilla1; } }
        public string CurrentFile { get { return currentFile; } }
        public EditorType Type { get { return EditorType.TextEditor; } }

        public FrmCodeEditor()
        {
            InitializeComponent();

            // Suscribir handlers básicos del formulario
            this.Load += MainForm_Load;
            // No suscribimos aquí Scintilla event handlers hasta que se configure el lenguaje
        }

        public override void ApplyTranslations()
        {
            base.ApplyTranslations();
            int line = scintilla1.LineFromPosition(scintilla1.CurrentPosition) + 1;
            int col = scintilla1.GetColumn(scintilla1.CurrentPosition) + 1;
            if (toolStripStatusLabel1 != null)
                toolStripStatusLabel1.Text = $"{translator.Get("Format")}: {(translator.Get(currentLanguage?.Name ?? "—"))}  |  {translator.Get("Row")}: {line} {translator.Get("Col")}: {col}";
            UpdateTitle();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RegisterLanguages();
        }

        private void RegisterLanguages()
        {
            // Registrar los lenguajes disponibles
            var lua = new LuaLanguage();
            registeredLanguages[lua.Name] = lua;

            var textPlain = new TextPlainLanguage();
            registeredLanguages[textPlain.Name] = textPlain;

            // Aquí podrías registrar más lenguajes:
            // var cs = new CSharpLanguage(); registeredLanguages[cs.Name] = cs;

            // Seleccionar lenguaje por defecto
            SetLanguage(textPlain.Name);
        }

        private void SetLanguage(string languageName)
        {
            if (!registeredLanguages.TryGetValue(languageName, out var lang))
                return;

            // Quitar manejadores previos para evitar doble-suscripción
            try
            {
                scintilla1.CharAdded -= Scintilla1_CharAdded;
                scintilla1.UpdateUI -= Scintilla1_UpdateUI;
                scintilla1.KeyDown -= Scintilla1_KeyDown;
            }
            catch { /* ignore */ }

            currentLanguage = lang;

            // Delegar la configuración específica del lenguaje al objeto lenguaje
            lang.ConfigureScintilla(scintilla1);

            // Suscribir los manejadores comunes sobre el control ya configurado
            scintilla1.CharAdded += Scintilla1_CharAdded;
            scintilla1.UpdateUI += Scintilla1_UpdateUI;
            scintilla1.KeyDown += Scintilla1_KeyDown;

            // Actualizar UI
            UpdateTitle();
            if (toolStripStatusLabel1 != null)
                toolStripStatusLabel1.Text = $"{translator.Get("Format")}: {(translator.Get(lang.Name))}";
        }

        public void UpdateTitle()
        {
            string title = "";
            if(this.CurrentFile != null)
            {
                title = $"{Path.GetFileName(this.CurrentFile)}";
                this.DockHandler.ToolTipText = currentFile;
            }
            else if(currentLanguage != null)
            {
                title = $"{translator.Get("Editor")} - {(translator.Get(currentLanguage.Name))}";
                this.DockHandler.ToolTipText = "";
            }
            else
            {
                title = $"{translator.Get("Editor")}";
                this.DockHandler.ToolTipText = "";
            }
            if (scintilla1.Modified)
            {
                title = "* " + title;
            }
            this.Text = title;
        }

        public void UpdateStatusLabel()
        {
            int line = scintilla1.LineFromPosition(scintilla1.CurrentPosition) + 1;
            int col = scintilla1.GetColumn(scintilla1.CurrentPosition) + 1;
            if (toolStripStatusLabel1 != null)
                toolStripStatusLabel1.Text = $"{translator.Get("Format")}: {(translator.Get(currentLanguage?.Name ?? "—"))}  |  {translator.Get("Row")}: {line} {translator.Get("Col")}: {col}";
        }

        // ---------------- Handlers Scintilla / Autocomplete / CallTips ----------------

        private void Scintilla1_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            var s = scintilla1;
            if (s == null) return;

            UpdateStatusLabel();
            UpdateTitle();

            // Brace matching (intenta resaltar pares)
            var pos = s.CurrentPosition;
            if (pos >= 0 && pos <= s.TextLength)
            {
                int bracePos1 = -1;
                int bracePos2 = -1;
                char ch = (pos > 0) ? (char)s.GetCharAt(pos - 1) : '\0';
                if ("()[]{}".IndexOf(ch) >= 0)
                    bracePos1 = pos - 1;
                else
                {
                    char ch2 = (pos < s.TextLength) ? (char)s.GetCharAt(pos) : '\0';
                    if ("()[]{}".IndexOf(ch2) >= 0)
                        bracePos1 = pos;
                }

                if (bracePos1 >= 0)
                {
                    try
                    {
                        bracePos2 = s.BraceMatch(bracePos1);
                        if (bracePos2 == Scintilla.InvalidPosition)
                            s.BraceBadLight(bracePos1);
                        else
                            s.BraceHighlight(bracePos1, bracePos2);
                    }
                    catch
                    {
                        // Algunas builds podrían tener diferencias; ignoramos fallos menores aquí
                    }
                }
                else
                {
                    try { s.BraceHighlight(-1, -1); } catch { }
                }
            }
            currentLanguage.ApplyFunctionNameStyling(scintilla1);
            currentLanguage.HighlightMatchingBrace(scintilla1);
        }

        private void Scintilla1_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+Space -> autocompletado manual
            if (e.Control && e.KeyCode == Keys.Space)
            {
                var s = scintilla1;
                var docWords = GetWordsFromDocument();
                var langWords = currentLanguage?.GetKeywordsForAutoComplete() ?? Enumerable.Empty<string>();
                var list = langWords.Concat(docWords)
                                    .Distinct(StringComparer.OrdinalIgnoreCase)
                                    .OrderBy(x => x)
                                    .Take(1000)
                                    .ToArray();
                var text = string.Join(" ", list);
                try { s.AutoCShow(0, text); } catch { }
                e.SuppressKeyPress = true;
            }

            // Ctrl+/ para comentar/descomentar (usa token del lenguaje si existe)
            if (e.Control && e.KeyCode == Keys.OemQuestion)
            {
                ToggleCommentSelection();
                e.SuppressKeyPress = true;
            }
        }

        private void Scintilla1_CharAdded(object sender, CharAddedEventArgs e)
        {
            var s = scintilla1;
            if (s == null) return;

            int added = e.Char;

            // Permitir que el lenguaje intercepte el CharAdded (si devuelve true, no hacemos el handler genérico)
            try
            {
                if (currentLanguage != null && currentLanguage.OnCharAdded(s, added))
                    return;
            }
            catch
            {
                // Ignorar excepciones del handler de lenguaje
            }

            char ch = (char)added;
            int pos = s.CurrentPosition; // posición después del caracter insertado

            // 1) Autocompletado automático: si escribes letra/dígito/_ y longitud >= 2
            if (char.IsLetterOrDigit(ch) || ch == '_')
            {
                int wordStart = 0;
                try { wordStart = s.WordStartPosition(pos - 1, true); } catch { wordStart = pos - 1; }
                int len = Math.Max(0, pos - wordStart);
                if (len >= 2)
                {
                    var langWords = currentLanguage?.GetKeywordsForAutoComplete() ?? Enumerable.Empty<string>();
                    var docWords = GetWordsFromDocument();
                    var list = langWords.Concat(docWords)
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        .OrderBy(x => x)
                                        .Take(800)
                                        .ToArray();
                    var text = string.Join(" ", list);
                    try { s.AutoCShow(len, text); } catch { }
                }
            }

            // 2) Envolver selección si existe
            int selStart = s.SelectionStart;
            int selLen = s.SelectionEnd - s.SelectionStart;
            if (selLen > 0)
            {
                string selected = s.GetTextRange(selStart, selLen);
                switch (ch)
                {
                    case '(':
                        s.ReplaceSelection("(" + selected + ")");
                        return;
                    case '[':
                        s.ReplaceSelection("[" + selected + "]");
                        return;
                    case '{':
                        s.ReplaceSelection("{" + selected + "}");
                        return;
                    case '"':
                    case '\'':
                        s.ReplaceSelection(ch + selected + ch.ToString());
                        return;
                }
            }

            // 3) Auto-cierre y calltips
            switch (ch)
            {
                case '(':
                    try
                    {
                        s.InsertText(pos, ")");
                        s.GotoPosition(pos);
                    }
                    catch { }

                    // Mostrar call tip si la palabra anterior tiene firma
                    try
                    {
                        int identEndPos = Math.Max(0, s.WordEndPosition(pos - 2, true)); // antes del '('
                        int identStartPos = s.WordStartPosition(identEndPos, true);
                        int nameLen = identEndPos - identStartPos;
                        if (nameLen > 0 && currentLanguage != null)
                        {
                            string name = s.GetTextRange(identStartPos, nameLen);
                            var sigs = currentLanguage.GetFunctionSignatures();
                            if (sigs != null && sigs.TryGetValue(name, out var sig))
                            {
                                try { s.CallTipShow(pos, sig); } catch { }
                            }
                        }
                    }
                    catch { }
                    break;

                case '[':
                    try { s.InsertText(pos, "]"); s.GotoPosition(pos); } catch { }
                    break;

                case '{':
                    try { s.InsertText(pos, "}"); s.GotoPosition(pos); } catch { }
                    break;

                case '"':
                    try
                    {
                        if (pos < s.TextLength && s.GetCharAt(pos) == '"') break;
                        s.InsertText(pos, "\"");
                        s.GotoPosition(pos);
                    }
                    catch { }
                    break;

                case '\'':
                    try
                    {
                        if (pos < s.TextLength && s.GetCharAt(pos) == '\'') break;
                        s.InsertText(pos, "'");
                        s.GotoPosition(pos);
                    }
                    catch { }
                    break;
            }
        }

        // ---------------- Utilidades del editor ----------------

        private string[] GetWordsFromDocument()
        {
            var txt = scintilla1.Text ?? "";
            var matches = Regex.Matches(txt, "[a-zA-Z_][a-zA-Z0-9_]{2,}");
            var words = matches.Cast<Match>().Select(m => m.Value)
                               .Distinct(StringComparer.OrdinalIgnoreCase)
                               .ToArray();
            return words;
        }

        private void ToggleCommentSelection()
        {
            var s = scintilla1;
            if (s == null) return;

            var start = s.SelectionStart;
            var end = s.SelectionEnd;
            var startLine = s.LineFromPosition(start);
            var endLine = s.LineFromPosition(Math.Max(end - 1, 0));

            string commentToken = currentLanguage?.LineCommentToken ?? "--"; // fallback "--"

            // Iterar de abajo a arriba
            for (int i = endLine; i >= startLine; i--)
            {
                var lineObj = s.Lines[i];
                var lineStart = lineObj.Position;
                var lineLen = lineObj.Length;
                var lineText = lineObj.Text ?? "";

                if (!string.IsNullOrEmpty(commentToken) && lineText.TrimStart().StartsWith(commentToken))
                {
                    int idx = lineText.IndexOf(commentToken);
                    if (idx >= 0)
                    {
                        string newLine = lineText.Remove(idx, commentToken.Length);
                        // Intentar ReplaceTarget/TargetStart si existe (más eficiente)
                        try
                        {
                            s.TargetStart = lineStart;
                            s.TargetEnd = lineStart + lineLen;
                            s.ReplaceTarget(newLine);
                        }
                        catch
                        {
                            // Fallback seguro: seleccionar la línea y reemplazar selección
                            try
                            {
                                s.SetSelection(lineStart, lineStart + lineLen);
                                s.ReplaceSelection(newLine);
                                // restaurar caret a inicio de línea (opcional)
                                s.SetEmptySelection(lineStart);
                            }
                            catch
                            {
                                // si no funciona, ignoramos (evita crash en builds raras)
                            }
                        }
                    }
                }
                else
                {
                    string newLine = commentToken + lineText;
                    try
                    {
                        s.TargetStart = lineStart;
                        s.TargetEnd = lineStart + lineLen;
                        s.ReplaceTarget(newLine);
                    }
                    catch
                    {
                        // Fallback: seleccionar la línea y reemplazar selección
                        try
                        {
                            s.SetSelection(lineStart, lineStart + lineLen);
                            s.ReplaceSelection(newLine);
                            s.SetEmptySelection(lineStart);
                        }
                        catch
                        {
                            // ignorar errores para mantener robustez
                        }
                    }
                }
            }
        }

        // ---------------- Abrir / Guardar ----------------

        public void OpenFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return;
            currentFile = filePath;
            scintilla1.Text = File.ReadAllText(currentFile);
            // Detectar lenguaje por extensión
            var ext = Path.GetExtension(currentFile).ToLower();
            var lang = registeredLanguages.Values
                .FirstOrDefault(l => l.FileExtensions.Contains(ext))
                ?? registeredLanguages["PlainText"];
            SetLanguage(lang.Name);
            scintilla1.EmptyUndoBuffer();
            scintilla1.SetSavePoint();
        }

        public void NewFile()
        {
            currentFile = null;
            scintilla1.Text = "";
            SetLanguage(registeredLanguages["PlainText"].Name); // por defecto
        }

        public void SaveFile()
        {
            if (string.IsNullOrEmpty(currentFile))
                SaveFileAs();
            else
            {
                File.WriteAllText(currentFile, scintilla1.Text ?? "", Encoding.UTF8);
                scintilla1.SetSavePoint();
                UpdateTitle();
            }
        }

        public void SaveFileAs()
        {
            using (var dlg = new SaveFileDialog())
            {
                dlg.FileName = currentFile != null ? Path.GetFileName(currentFile) : $"{translator.Get("untitled")}{currentLanguage.FileExtensions.First()}";
                dlg.Filter = $"{translator.Get("LoadModDialogFilterAllFIles")} (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    currentFile = dlg.FileName;
                    File.WriteAllText(currentFile, scintilla1.Text ?? "", Encoding.UTF8);
                    var ext = Path.GetExtension(currentFile).ToLower();
                    var lang = registeredLanguages.Values
                        .FirstOrDefault(l => l.FileExtensions.Contains(ext))
                        ?? registeredLanguages["PlainText"];
                    SetLanguage(lang.Name);
                    scintilla1.SetSavePoint();
                }
            }
        }

        public static void OpenFile()
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = $"{TranslationProvider.GetInstance().Get("LoadModDialogFilterAllFIles")} (*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    EditorManager.GetInstance().OpenEditor(dlg.FileName, EditorType.TextEditor);
                }
            }
        }


        public void ResetZoom()
        {
            try
            {
                scintilla1.Zoom = 0;
            }
            catch
            {
                // ignorar errores
            }
        }

        public void Use()
        {
            this.Focus();
            this.Activate();
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            undoToolStripMenuItem.Enabled = scintilla1.CanUndo;
            redoToolStripMenuItem.Enabled = scintilla1.CanRedo;
            cutToolStripMenuItem.Enabled = scintilla1.SelectedText.Length > 0;
            copyToolStripMenuItem.Enabled = scintilla1.SelectedText.Length > 0;
            pasteToolStripMenuItem.Enabled = Clipboard.ContainsText();
            selectAllToolStripMenuItem.Enabled = scintilla1.TextLength > 0;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scintilla1.CanUndo) scintilla1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scintilla1.CanUndo) scintilla1.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scintilla1.SelectedText.Length > 0) scintilla1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scintilla1.SelectedText.Length > 0) scintilla1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText()) scintilla1.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scintilla1.SelectAll();
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SearchSystem.GetInstance().OpenFindForActiveEditor();
        }
    }
}
