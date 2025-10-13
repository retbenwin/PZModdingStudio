using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PZModdingStudio.Forms
{
    public partial class FrmLUAEditor : FrmBase
    {
        public FrmLUAEditor()
        {
            InitializeComponent();
        }

        private void FrmLUAEditor_Load(object sender, EventArgs e)
        {
            scintilla.Margins[0].Width = 40;
            scintilla.Margins[1].Width = 0;
        }
    }
}
