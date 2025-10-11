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

        public FrmMainMenu()
        {
            isInitializing = true;
            InitializeComponent();
            isInitializing = false;

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSettings frm = new FrmSettings();
            frm.ShowDialog(this);
        }
    }
}
