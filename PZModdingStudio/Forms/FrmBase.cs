using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using PZModdingStudio.Lang;

namespace PZModdingStudio.Forms
{
    public partial class FrmBase : DockContent, ITranslatable
    {

        public TranslationProvider translator;
        public Config.Settings appSettings;
        public ModsManager modsManager;
        public FrmMainMenu ParentMenuForm { get; set; } = null;

        public bool VisibleLoadingPanelWhenLoading { get; set; } = false;

        public FrmBase()
        {
            InitializeComponent();
            if (!IsDesignMode())
            {
                // Evitar inicializar en tiempo de diseño para no cargar configuraciones ni traducciones
                appSettings = Config.Settings.Get();
                translator = TranslationProvider.GetInstance();
                modsManager = ModsManager.GetInstance();
                SetLoading(false);
            }
        }

        public virtual void ApplyTranslations()
        {
            translator.ApplyTranslations(this);
            lblLoading.Text = translator.Get("lblLoading.Text");
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
            if (!IsDesignMode())
                ApplyTranslations();
        }


        public void SetLoading(bool isLoading)
        {
            LoadingBehaviour(isLoading);
        }

        internal virtual void LoadingBehaviour(bool isLoading){
            if (VisibleLoadingPanelWhenLoading)
            {
                this.pnlLoading.BringToFront();
                int x = (this.ClientSize.Width - this.pnlLoading.Width) / 2;
                int y = (this.ClientSize.Height - this.pnlLoading.Height) / 2;
                this.pnlLoading.Location = new Point(x, y);
                Application.DoEvents();
            }
            this.pnlLoading.Visible = isLoading && this.VisibleLoadingPanelWhenLoading;
            foreach (Control c in this.Controls)
            {
                if (c == pnlLoading) continue;
                //Desactivar todos los paneles
                if (c is Panel || c is MenuStrip || c is SplitContainer || c is SplitterPanel || c is HiddenScrollPanel || c is FileExplorerTree)
                {
                    c.Enabled = !isLoading;
                }
            }
        }

        internal bool RequiredSelectedMod()
        {
            if (modsManager.Mods.Count == 0 || modsManager.SelectedMod == null)
            {
                MessageBox.Show(this, translator.Get("MsgSelectModFirst"), translator.Get("MsgSelectModFirstTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }

        private void FrmBase_SizeChanged(object sender, EventArgs e)
        {
            if (VisibleLoadingPanelWhenLoading)
            {
                this.pnlLoading.BringToFront();
                int x = (this.ClientSize.Width - this.pnlLoading.Width) / 2;
                int y = (this.ClientSize.Height - this.pnlLoading.Height) / 2;
                this.pnlLoading.Location = new Point(x, y);
            }
        }


    }

}
