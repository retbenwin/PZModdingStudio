using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PZModdingStudio.Forms
{
    public partial class FrmMainMenu : FrmBase
    {

        private bool isInitializing = true;

        private ModsManager modsManager;

        public FrmMainMenu()
        {
            isInitializing = true;
            InitializeComponent();
            modsManager = ModsManager.GetInstance();
            PopulateComboMods();
            isInitializing = false;

        }

        private void PopulateComboMods()
        {
            cboModList.DataSource = modsManager.Mods;
            if (cboModList.Items.Count > 0)
            {
                cboModList.SelectedIndex = 0;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings frm = new FrmSettings();
            frm.ShowDialog(this);
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isInitializing) return;
            modsManager.LoadMod();
        }
    }
}
