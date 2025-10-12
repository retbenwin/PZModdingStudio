using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PZModdingStudio.PZTypes
{
    internal class Mod
    {
        public Mod() { }

        public Mod(ModInfo modInfo) {
            this.ModInfo = modInfo;
        }

        public ModInfo ModInfo { get; set; }

        public override string ToString()
        {
            return ModInfo.id;
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(ModInfo.id) && !string.IsNullOrWhiteSpace(ModInfo.name);
        }

    }
}
