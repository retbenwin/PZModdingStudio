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
    public partial class FrmSettings : FrmBase
    {
        private readonly bool isInitializing = true;

        public FrmSettings()
        {
            isInitializing = true;
            InitializeComponent();
            PopulateLanguagesCombo();
            isInitializing = false;
        }

        private void PopulateLanguagesCombo()
        {
            var codes = translator.GetAvailableLanguages().ToArray(); // asume que devuelve "en","es",...
            var items = codes.Select(c => new LanguageItem { Code = c, DisplayName = LanguageItem.GetDisplayNameForCultureCode(c) })
                             .OrderBy(li => li.DisplayName) // opcional: ordenar alfabéticamente
                             .ToList();

            comboLanguages.DisplayMember = "DisplayName";
            comboLanguages.ValueMember = "Code";
            comboLanguages.DataSource = items;

            // seleccionar el idioma actual si está disponible
            if (!string.IsNullOrEmpty(translator.CurrentLanguage))
                comboLanguages.SelectedValue = translator.CurrentLanguage;
        }

        private void ApplyChanges()
        {
            if (comboLanguages.SelectedValue == null) return;
            string newLang = comboLanguages.SelectedValue.ToString();
            if (translator.SetLanguage(newLang))
            {
                appSettings.Language = newLang;
                Application.OpenForms.OfType<FrmBase>().ToList().ForEach(f =>
                    f.ApplyTranslations()
                );
            }
            appSettings.Save();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (isInitializing) return;
            ApplyChanges();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (isInitializing) return;
            ApplyChanges();
            this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (isInitializing) return;
            this.Close();
        }
    }
}
