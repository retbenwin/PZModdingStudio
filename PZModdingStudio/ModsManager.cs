using PZModdingStudio.PZTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PZModdingStudio
{
    public class ModsManager
    {

        private static ModsManager instance = null;
        private TranslationProvider translator;
        public BindingList<Mod> Mods { get; set; } = null;
        public Mod SelectedMod { get; set; } = null;

        private ModsManager() {
            Mods = new BindingList<Mod>();
            translator = TranslationProvider.GetInstance();
        }   


        public static ModsManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ModsManager();
            }
            return instance;
        }

        public Boolean LoadMod()
        {
            string filePath = "";
            FileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = translator.Get("LoadModDialog");
            fileDialog.Filter = translator.Get("LoadModDialogFilter") + " (*.info)|*.info|" + translator.Get("LoadModDialogFilterAllFIles") + " (*.*)|*.*";
            fileDialog.SupportMultiDottedExtensions = true;
            fileDialog.RestoreDirectory = true;
            fileDialog.CheckFileExists = true;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = fileDialog.FileName;
            }
            else
            {
                return false;
            }
            return LoadMod(filePath);
        }

        public Boolean LoadMod(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;
            if (!File.Exists(filePath))
            {
                return false;
            }
            ModInfo modInfo = InfoFile.Deserialize<ModInfo>(filePath, false, true);
            if (modInfo == null)
            {
                return false;
            }
            modInfo.SetFilePath(filePath);
            Mod mod = new Mod(modInfo);

            if (!mod.IsValid())
            {
                MessageBox.Show(translator.Get("LoadModInvalidModFile"), translator.Get("Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            //Comprobar que no esté ya cargado
            int repeatedNumber = 0;
            foreach (Mod m in Mods)
            {
                if (m.ModInfo.id == mod.ModInfo.id)
                {
                    repeatedNumber = m.GetRepeatedId() + 1;
                    if (m.ModInfo.GetFilePath() == mod.ModInfo.GetFilePath())
                    {
                        MessageBox.Show(string.Format(translator.Get("LoadModAlreadyLoaded"), mod.ModInfo.id), translator.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }

            if (repeatedNumber > 0)
            {
                mod.SetRepeatedNumber(repeatedNumber);
            }

            string folderPath = Path.GetDirectoryName(filePath);
            var dir = new DirectoryInfo(folderPath);
            string workSpacePath = dir.Parent?.FullName;
            mod.SetWorkspacePath(workSpacePath);

            Mods.Add(mod);
            return true;
        }

        public Boolean UnloadMod(Mod mod)
        {
            if (mod == null) return false;
            return Mods.Remove(mod);
        }

    }
}
