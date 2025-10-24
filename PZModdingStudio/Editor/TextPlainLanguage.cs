using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PZModdingStudio.Editor
{

    public class TextPlainLanguage : ILanguageDefinition
    {
        public string Name => "Texto plano";

        // Extensiones sin punto (para compatibilidad con la comprobación que uses)
        public IEnumerable<string> FileExtensions => new[] { ".txt" };

        public string LineCommentToken => null; // no aplica

        public void ConfigureScintilla(Scintilla scintilla)
        {
            if (scintilla == null) return;

            // Sin lexer (texto plano)
            try { scintilla.Lexer = Lexer.Null; } catch { }

            // Fuente y estilo básico
            try
            {
                scintilla.StyleResetDefault();
                scintilla.Styles[Style.Default].Font = "Consolas";
                scintilla.Styles[Style.Default].Size = 11;
                scintilla.StyleClearAll();
            }
            catch { }

            // Margen de líneas y wrap
            try
            {
                scintilla.Margins[0].Width = 30;
                scintilla.WrapMode = WrapMode.None;
            }
            catch { }

            // Selección y caret
            try
            {
                scintilla.CaretForeColor = Color.Black;
                scintilla.SetSelectionBackColor(true, Color.FromArgb(220, 220, 255));
            }
            catch { }

            // Desactivar/evitar propiedades de autocompletado si existen (compatibilidad)
            try { scintilla.AutoCSeparator = (char)0; } catch { }
            try { scintilla.AutoCCancelAtStart = true; } catch { }

            // Importante: no suscribimos handlers específicos aquí; MainForm suscribe CharAdded
            // pero el OnCharAdded de este lenguaje devolverá true para prevenir autocompletado.
        }

        public IEnumerable<string> GetKeywordsForAutoComplete()
        {
            // No hay palabras clave para texto plano
            return new string[0];
        }

        public Dictionary<string, string> GetFunctionSignatures()
        {
            // Sin firmas para texto plano
            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Interceptamos CharAdded y devolvemos true para indicar que hemos manejado el evento
        /// y que el MainForm no debe ejecutar el comportamiento genérico (autocompletado).
        /// </summary>
        public bool OnCharAdded(Scintilla scintilla, int addedChar)
        {
            // No hacemos nada y retornamos true para bloquear el autocompletado genérico.
            return true;
        }

        IDictionary<string, string> ILanguageDefinition.GetFunctionSignatures()
        {
            return GetFunctionSignatures();
        }

        public void ApplyFunctionNameStyling(Scintilla s)
        {
            // Sin estilos específicos para texto plano
        }

        public void HighlightMatchingBrace(Scintilla s)
        {
            
        }
    }

}
