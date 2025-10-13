using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZModdingStudio.PZTypes
{
    public class ModInfo
    {

        private string filePath = "";

        public string name { get; set; } = "";
        public string id { get; set; } = "";
        public string author { get; set; } = "";
        public string description { get; set; } = "";
        public string poster { get; set; } = "";
        public string modversion { get; set; } = "";
        public string versionurl { get; set; } = "";
        public string url { get; set; } = "";
        public string icon { get; set; } = "";

        public void SetFilePath(string path)
        {
            filePath = path;
        }

        public string GetFilePath()
        {
            return filePath;
        }

    }
}
