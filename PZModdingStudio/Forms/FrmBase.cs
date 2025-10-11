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
    public partial class FrmBase : Form
    {

        public TranslationProvider translator;
        public Config.Settings appSettings;

        public FrmBase()
        {
            InitializeComponent();
            if (!IsDesignMode())
            {
                // Evitar inicializar en tiempo de diseño para no cargar configuraciones ni traducciones
                appSettings = Config.Settings.Get();
                translator = TranslationProvider.GetInstance();
            }
        }

        public void ApplyTranslations()
        {
            translator.ApplyTranslations(this);
        }

        private bool IsDesignMode()
        {
            if (System.ComponentModel.LicenseManager.UsageMode ==
                System.ComponentModel.LicenseUsageMode.Designtime)
                return true;
            try
            {
                var name = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                return name.IndexOf("devenv", StringComparison.OrdinalIgnoreCase) >= 0 ||
                       name.IndexOf("XDesProc", StringComparison.OrdinalIgnoreCase) >= 0;
            }
            catch { return false; }
        }

        private void FrmBase_Load(object sender, EventArgs e)
        {
            if(!IsDesignMode())
                ApplyTranslations();
        }
    }
}
