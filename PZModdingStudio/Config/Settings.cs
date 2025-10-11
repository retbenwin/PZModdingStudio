using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PZModdingStudio.Config
{
    public class Settings
    {
        // Serializar esta clase a un archivo JSON
        private static string _basePath = Path.Combine(Application.StartupPath, "Config");
        private static string _filePath =  Path.Combine(_basePath, "settings.json");

        private static Settings _instance;

        public string Language { get; set; } = "en"; // Idioma por defecto es inglés

        public static Settings Load()
        {
            if (!System.IO.File.Exists(_filePath))
            {   
                return (new Settings()).Save(); // Devuelve configuración por defecto si no existe el archivo
            }
            var json = System.IO.File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<Settings>(json);
        }

        public Settings Save()
        {
            if(!Directory.Exists(_basePath)) {
                Directory.CreateDirectory(_basePath);
            }
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            System.IO.File.WriteAllText(_filePath, json);
            return this;
        }

        public static Settings Get() {
            if(_instance == null) {
                _instance = Load();
            }
            return _instance;
        }


    }
}
