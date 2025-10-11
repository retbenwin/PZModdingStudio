using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZModdingStudio
{
    internal class LanguageItem
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public override string ToString() => DisplayName;


        public static string GetDisplayNameForCultureCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return code;
            try
            {
                var ci = new System.Globalization.CultureInfo(code);
                // Preferimos NativeName ("Español", "English"). Si está vacío, usamos EnglishName.
                var name = string.IsNullOrWhiteSpace(ci.NativeName) ? ci.EnglishName : ci.NativeName;
                return CapitalizeFirstLetter(name);
            }
            catch (ArgumentException)
            {
                // Si el código no es un culture válido, devuelvo el código tal cual (o podrías mapearlo manualmente).
                return code;
            }
        }

        public static string CapitalizeFirstLetter(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s.Substring(1);
        }


    }
}
