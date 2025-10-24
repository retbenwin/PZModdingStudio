using System.Collections.Generic;
using ScintillaNET;

namespace PZModdingStudio.Editor
{

    public interface ILanguageDefinition
    {
        /// <summary>Nombre legible del lenguaje (ej. "Lua").</summary>
        string Name { get; }

        /// <summary>Extensiones típicas (sin punto) asociadas al lenguaje (ej. "lua").</summary>
        IEnumerable<string> FileExtensions { get; }

        /// <summary>Configura el control Scintilla para este lenguaje (lexer, estilos, márgenes, propiedades).</summary>
        void ConfigureScintilla(Scintilla scintilla);

        /// <summary>Lista de palabras clave / builtins que el lenguaje quiere usar para autocompletado.</summary>
        IEnumerable<string> GetKeywordsForAutoComplete();

        /// <summary>Diccionario de firmas para call-tips: key = nombre función, value = texto a mostrar.</summary>
        IDictionary<string, string> GetFunctionSignatures();

        /// <summary>Token de comentario de línea (ej: \"--\" para Lua, \"//\" para C#). Puede ser null si no aplica.</summary>
        string LineCommentToken { get; }

        /// <summary>Opcional: manejar CharAdded de forma específica. Si devuelve true, el MainForm no ejecutará el handler global adicional.</summary>
        bool OnCharAdded(Scintilla scintilla, int addedChar);

        void ApplyFunctionNameStyling(Scintilla s);

        void HighlightMatchingBrace(Scintilla s);


    }

}
