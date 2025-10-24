using System;
using System.Collections.Generic;
using System.Drawing;
using ScintillaNET;

namespace PZModdingStudio.Editor
{

    public class LuaLanguage : ILanguageDefinition
    {
        public string Name => "Lua";

        public IEnumerable<string> FileExtensions => new[] { ".lua" };

        public string LineCommentToken => "--";

        private int _functionsLastHash = 0;
        private const int FUNCTION_INDICATOR = 10; // índice del indicator que usaremos
        private const int SELF_INDICATOR = 11;     // nuevo: indicador para 'self'

        public void ConfigureScintilla(Scintilla s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));

            // Reset y fuente
            s.StyleResetDefault();
            s.Styles[Style.Default].Font = "Consolas";
            s.Styles[Style.Default].Size = 10;
            s.StyleClearAll();

            // Lexer Lua
            s.Lexer = Lexer.Lua;

            // Estilos básicos de Lua
            s.Styles[Style.Lua.Default].ForeColor = Color.Silver;
            s.Styles[Style.Lua.Comment].ForeColor = Color.FromArgb(0, 128, 0);
            s.Styles[Style.Lua.CommentLine].ForeColor = Color.FromArgb(0, 128, 0);
            s.Styles[Style.Lua.Number].ForeColor = Color.Green;
            s.Styles[Style.Lua.Word].ForeColor = Color.Blue; // keywords
            s.Styles[Style.Lua.String].ForeColor = Color.FromArgb(163, 21, 21);
            s.Styles[Style.Lua.Character].ForeColor = Color.FromArgb(163, 21, 21);
            s.Styles[Style.Lua.Operator].ForeColor = Color.Purple;

            // Plegado: propiedades generales
            try
            {
                s.SetProperty("fold", "1");
                s.SetProperty("fold.compact", "1");
            }
            catch { /* algunas builds pueden ignorarlo */ }

            // Márgenes - números de línea
            try
            {
                s.Margins[0].Type = MarginType.Number;
                s.Margins[0].Width = 40;
            }
            catch { }

            // Márgen para plegado (usamos margen 2)
            const int FOLD_MARGIN = 2;
            try
            {
                s.Margins[FOLD_MARGIN].Type = MarginType.Symbol;
                s.Margins[FOLD_MARGIN].Mask = Marker.MaskFolders;
                s.Margins[FOLD_MARGIN].Sensitive = true;
                s.Margins[FOLD_MARGIN].Width = 20;
            }
            catch { }

            // Configurar marcadores de folder (símbolos +/-, líneas de conexión)
            try
            {
                // Colores de marcadores (primer intento)
                for (int m = Marker.FolderEnd; m <= Marker.FolderOpen; m++)
                {
                    try
                    {
                        s.Markers[m].SetForeColor(Color.White);
                        s.Markers[m].SetBackColor(Color.Gray);
                    }
                    catch { /* algunas versiones no exponen SetForeColor/SetBackColor */ }
                }

                s.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                s.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                s.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                s.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                s.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                s.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                s.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;
            }
            catch { }

            // Resaltar línea actual
            try
            {
                s.CaretLineVisible = true;
                s.CaretLineBackColor = Color.FromArgb(240, 240, 240);
            }
            catch { }

            // Keywords: se asignan a los slots de Scintilla
            var luaKeywords = "and break do else elseif end false for function goto if in local nil not or repeat return then true until while self";
            try { s.SetKeywords(0, luaKeywords); } catch { }

            var luaBuiltins = "assert collectgarbage dofile error _G _VERSION ipairs load loadfile next pairs pcall print rawequal rawget rawlen rawset require select setmetatable tonumber tostring type xpcall coroutine string table math io os debug package";
            try { s.SetKeywords(1, luaBuiltins); } catch { }

            // Autocompletado: separador y comportamiento (compatibilidad)
            try { s.AutoCSeparator = (char)32; } catch { }
            try { s.AutoCCancelAtStart = false; } catch { }

            // Intentar habilitar AutomaticFold si está soportado (varía por versión)
            try
            {
                s.AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;
            }
            catch
            {
                // Ignorar si la propiedad no existe en esta build
            }

            // Opcional: configurar el color de fondo del margen de plegado para que combine
            try
            {
                s.SetFoldMarginColor(true, Color.FromArgb(245, 245, 245));
                s.SetFoldMarginHighlightColor(true, Color.FromArgb(245, 245, 245));
            }
            catch { }


            try
            {
                // Configurar indicador para nombres de función (estilo: color del texto)
                s.Indicators[FUNCTION_INDICATOR].Style = IndicatorStyle.TextFore;
                s.Indicators[FUNCTION_INDICATOR].Under = false;
                // color que prefieras:
                s.Indicators[FUNCTION_INDICATOR].ForeColor = ColorTranslator.FromHtml("#637d0f");
                // Iniciar limpia
                //try { s.IndicatorClearRange(0, s.TextLength); } catch { }
            }
            catch
            {
                // ignorar si la build no tiene Indicator... (poco probable)
            }

            // Indicator 0 -> resaltar los caracteres (corchetes)
            var i0 = s.Indicators[0];
            i0.Style = ScintillaNET.IndicatorStyle.RoundBox; // u otro: Box, StraightBox, etc.
            i0.Under = true;
            i0.Alpha = 200; // transparencia
            i0.ForeColor = System.Drawing.Color.LightSkyBlue;

            // Indicator 1 -> rellenar el rango entre los corchetes (opcional)
            var i1 = s.Indicators[1];
            i1.Style = ScintillaNET.IndicatorStyle.FullBox;
            i1.Under = true;
            i1.Alpha = 50;
            i1.ForeColor = System.Drawing.Color.White;

            // Configurar indicador independiente para 'self'
            s.Indicators[SELF_INDICATOR].Style = IndicatorStyle.TextFore; // o RoundBox si prefieres background
            s.Indicators[SELF_INDICATOR].Under = false;
            s.Indicators[SELF_INDICATOR].Alpha = 255;
            s.Indicators[SELF_INDICATOR].ForeColor = Color.Blue;

        }

        public IEnumerable<string> GetKeywordsForAutoComplete()
        {
            var luaKeywords = "and break do else elseif end false for function goto if in local nil not or repeat return then true until while";
            var luaBuiltins = "assert collectgarbage dofile error _G _VERSION ipairs load loadfile next pairs pcall print rawequal rawget rawlen rawset require select setmetatable tonumber tostring type xpcall coroutine string table math io os debug package";
            var all = (luaKeywords + " " + luaBuiltins).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return all;
        }

        public IDictionary<string, string> GetFunctionSignatures()
        {
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "print", "print(value [, ...]) - Imprime valores en la salida estándar" },
                { "tonumber", "tonumber(e [, base]) - Convierte a número" },
                { "tostring", "tostring(v) - Convierte a cadena" },
                { "pairs", "pairs(t) - Iterador para tablas" },
                { "ipairs", "ipairs(t) - Iterador por índices numéricos" },
                { "math.sin", "math.sin(x) - Seno (x en radianes)" },
                { "math.cos", "math.cos(x) - Coseno" },
                { "table.insert", "table.insert(t, [pos,] value) - Inserta en tabla" },
                { "table.remove", "table.remove(t [, pos]) - Elimina de tabla" },
                { "string.match", "string.match(s, pattern) - Busca patrón" },
                { "io.open", "io.open(filename, mode) - Abre archivo" },

            };
            return d;
        }

        // === Helper: comprobar si una posición está dentro de un comentario (incluye --[[ ... ]]) ===
        private bool IsPositionInComment(Scintilla sc, int position)
        {
            try
            {
                int style = sc.GetStyleAt(position);
                if (style == (int)Style.Lua.Comment || style == (int)Style.Lua.CommentLine ||
                    style == (int)Style.Lua.CommentDoc)
                    return true;
            }
            catch
            {
                // Ignorar si la API no soporta GetStyleAt
            }

            try
            {
                // --- Fallback textual ---
                // Detectar si hay '--' antes en la línea (comentario simple)
                int line = sc.LineFromPosition(position);
                int lineStart = sc.Lines[line].Position;
                int relPos = position - lineStart;
                if (relPos > 0)
                {
                    string lineText = sc.GetTextRange(lineStart, sc.Lines[line].Length) ?? "";
                    int commentIdx = lineText.IndexOf("--", StringComparison.Ordinal);
                    if (commentIdx >= 0 && commentIdx < relPos)
                        return true;
                }

                // Detectar si estamos dentro de un bloque --[[ ... ]]
                string textUpToPos = sc.GetTextRange(0, position);
                int openIdx = textUpToPos.LastIndexOf("--[[", StringComparison.Ordinal);
                if (openIdx >= 0)
                {
                    int closeIdx = textUpToPos.LastIndexOf("]]", StringComparison.Ordinal);
                    // Si hay un --[[ sin cierre antes de esta posición → seguimos dentro del bloque
                    if (closeIdx < openIdx)
                        return true;
                }
            }
            catch { }

            return false;
        }

        private bool IsRangeInComment(Scintilla sc, int start, int length)
        {
            if (sc == null) return false;
            if (length <= 0) return false;
            int end = start + length - 1;
            try
            {
                for (int p = start; p <= end; p++)
                {
                    if (!IsPositionInComment(sc, p))
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Manejo simple de CharAdded por el lenguaje. Retorna false para permitir que MainForm haga el manejo genérico.
        /// Puedes sobrescribir esto si quieres interceptar caracteres específicos.
        /// </summary>
        public bool OnCharAdded(Scintilla scintilla, int addedChar)
        {
            // No interceptamos nada a nivel de lenguaje; MainForm hace el comportamiento común (autocompl, calltips, pares).
            return false;
        }

        /// <summary>
        /// Busca definiciones de funciones en el texto y aplica un indicator sobre el nombre.
        /// Llamar periódicamente (por ejemplo desde MainForm.UpdateUI) — el método internamente evita
        /// recomputar si el texto no cambió (hash rápido).
        /// </summary>
        public void ApplyFunctionNameStyling(Scintilla s)
        {
            if (s == null) return;

            string txt = s.Text ?? "";
            int h = txt.GetHashCode();
            if (h == _functionsLastHash) return;
            _functionsLastHash = h;

            // Asegurarse de trabajar sobre el indicador correcto
            s.IndicatorCurrent = FUNCTION_INDICATOR;
            try { s.IndicatorClearRange(0, txt.Length); } catch { }

            if (string.IsNullOrEmpty(txt)) return;

            var rxDef = new System.Text.RegularExpressions.Regex(
                @"\b(?:local\s+)?function\s+([A-Za-z_][A-Za-z0-9_:\.]*)",
                System.Text.RegularExpressions.RegexOptions.Compiled);

            var matchesDef = rxDef.Matches(txt);
            foreach (System.Text.RegularExpressions.Match m in matchesDef)
            {
                if (m.Groups.Count < 2) continue;
                var g = m.Groups[1];
                string fullName = g.Value;
                int idx = g.Index;
                int len = g.Length;

                if (len <= 0) continue;
                int lastSep = fullName.LastIndexOfAny(new[] { '.', ':' });
                int realIdx = (lastSep >= 0) ? idx + lastSep + 1 : idx;
                int realLen = (lastSep >= 0) ? fullName.Length - (lastSep + 1) : len;
                if (realLen <= 0) continue;

                if (IsPositionInComment(s, realIdx)) continue;

                // pintar con el indicator correcto
                try
                {
                    s.IndicatorCurrent = FUNCTION_INDICATOR;
                    s.IndicatorFillRange(realIdx, realLen);
                }
                catch { }
            }

            // === llamadas ===
            var rxCall = new System.Text.RegularExpressions.Regex(
                @"(?<!\bfunction\s)\b([A-Za-z_][A-Za-z0-9_:\.]*)\s*(?=\()",
                System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.Singleline);

            var keywordSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "and","break","do","else","elseif","end","false","for","function","goto","if","in","local",
        "nil","not","or","repeat","return","then","true","until","while"
    };

            var matchesCall = rxCall.Matches(txt);
            foreach (System.Text.RegularExpressions.Match m in matchesCall)
            {
                if (m.Groups.Count < 1) continue;
                var g = m.Groups[1];
                string fullName = g.Value;
                if (string.IsNullOrWhiteSpace(fullName)) continue;

                int lastSep = fullName.LastIndexOfAny(new[] { '.', ':' });
                string lastPart = (lastSep >= 0 && lastSep + 1 < fullName.Length)
                    ? fullName.Substring(lastSep + 1)
                    : fullName;
                if (keywordSet.Contains(lastPart)) continue;

                int idx = g.Index;
                int len = g.Length;
                int realIdx = (lastSep >= 0) ? idx + lastSep + 1 : idx;
                int realLen = (lastSep >= 0) ? fullName.Length - (lastSep + 1) : len;
                if (realLen <= 0) continue;

                if (IsPositionInComment(s, realIdx)) continue;
                try
                {
                    s.IndicatorCurrent = FUNCTION_INDICATOR;
                    s.IndicatorFillRange(realIdx, realLen);
                }
                catch { }
            }

            // --- Resaltar 'self' cuando aparece como prefijo (self.xxx o self:xxx) ---
            try
            {
                s.IndicatorCurrent = SELF_INDICATOR;
                var rxSelfPrefix = new System.Text.RegularExpressions.Regex(@"\bself\b(?=[\.:])",
                    System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                foreach (System.Text.RegularExpressions.Match m in rxSelfPrefix.Matches(txt))
                {
                    if (m.Index >= 0 && m.Length > 0)
                    {
                        if (IsRangeInComment(s, m.Index, m.Length)) continue;
                        s.IndicatorFillRange(m.Index, m.Length);
                    }
                }
            }
            catch { }

        }


        public void HighlightMatchingBrace(Scintilla s)
        {
            if (s == null) return;

            int pos = s.CurrentPosition;
            int textLen = s.TextLength;

            // limpiar indicadores anteriores (no laves aquí todo el documento si es muy grande,
            // pero para simplicidad lo borramos todo)
            s.IndicatorCurrent = 0;
            s.IndicatorClearRange(0, textLen);
            // limpiar highlight integrado también (por si llega a usarse)
            try { s.BraceHighlight(-1, -1); } catch { }
            try { s.BraceBadLight(-1); } catch { }

            if (textLen == 0) return;

            int bracePos = -1;
            if (pos > 0)
            {
                char c = (char)s.GetCharAt(pos - 1);
                if ("{}()[]".IndexOf(c) >= 0) bracePos = pos - 1;
            }
            if (bracePos == -1 && pos < textLen)
            {
                char c = (char)s.GetCharAt(pos);
                if ("{}()[]".IndexOf(c) >= 0) bracePos = pos;
            }

            if (bracePos == -1) return;

            int matchPos = s.BraceMatch(bracePos);

            if (matchPos != -1)
            {
                // resaltar los caracteres usando indicator 0
                s.IndicatorCurrent = 0;
                s.IndicatorFillRange(bracePos, 1);
                s.IndicatorFillRange(matchPos, 1);

                // opcional: resaltar el rango interior usando indicator 1 (como VS Code)
                int start = Math.Min(bracePos, matchPos) + 1;
                int len = Math.Abs(matchPos - bracePos) - 1;
                if (len > 0)
                {
                    s.IndicatorCurrent = 1;
                    s.IndicatorFillRange(start, len);
                }
            }
            else
            {
                // si no hay match, podemos usar BraceBadLight o marcar con indicador rojo
                try { s.BraceBadLight(bracePos); } catch { }
                // ejemplo alternativo con indicador rojo:
                // s.Indicators[2].ForeColor = Color.FromArgb(...); // configurar indicador 2 si quieres
                // s.IndicatorCurrent = 2; s.IndicatorFillRange(bracePos, 1);
            }
        }


    }

}
