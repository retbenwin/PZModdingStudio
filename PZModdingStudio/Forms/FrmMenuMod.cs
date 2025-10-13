using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PZModdingStudio.Forms
{
    public partial class FrmMenuMod : FrmBase
    {

        public FrmMenuMod()
        {
            InitializeComponent();
        }

        private void btn3DVisor_Click(object sender, EventArgs e)
        {
            if (RequiredSelectedMod()) return;
        }

        private void btnLUA_Click(object sender, EventArgs e)
        {
            if (RequiredSelectedMod()) return;
            FrmLUAEditor frm = new FrmLUAEditor();
            frm.ParentForm = this.ParentForm;
            frm.Show(this.ParentForm.MainDockPanel, DockState.Document);
        }
    }
}
