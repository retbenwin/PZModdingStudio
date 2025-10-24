using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PZModdingStudio.Forms
{
    public partial class FrmFind : FrmBase
    {

        private Scintilla _scintilla;

        public FrmFind()
        {
            InitializeComponent();
        }

        private void TxtFind_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) { FindNext(); e.Handled = true; }
        }

        public void FocusFindTextBox()
        {
            txtFind.Focus();
            txtFind.SelectAll();
        }

        public void AttachScintilla(Scintilla scintilla)
        {
            _scintilla = scintilla;
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            FindPrevious();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            FindNext();
        }

        private void FindNext()
        {
            if (_scintilla == null) return;
            string pattern = txtFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;

            // Configure flags
            var flags = SearchFlags.None;
            if (chkMatchCase.Checked) flags |= SearchFlags.MatchCase;
            if (chkWholeWord.Checked) flags |= SearchFlags.WholeWord;
            if (chkRegex.Checked) flags |= SearchFlags.Regex;

            // Start search after current selection end
            int startSearch = _scintilla.SelectionEnd;
            _scintilla.TargetStart = startSearch;
            _scintilla.TargetEnd = _scintilla.TextLength;
            _scintilla.SearchFlags = flags;

            int found = _scintilla.SearchInTarget(pattern);

            if (found != -1)
            {
                _scintilla.SetSelection(found, found + (_scintilla.TargetEnd - _scintilla.TargetStart == 0 ? pattern.Length : _scintilla.TargetEnd - _scintilla.TargetStart));
                _scintilla.ScrollCaret();
            }
            else if (chkWrap.Checked)
            {
                // wrap: search from start
                _scintilla.TargetStart = 0;
                _scintilla.TargetEnd = startSearch;
                _scintilla.SearchFlags = flags;
                found = _scintilla.SearchInTarget(pattern);
                if (found != -1)
                {
                    _scintilla.SetSelection(found, found + (_scintilla.TargetEnd - _scintilla.TargetStart == 0 ? pattern.Length : _scintilla.TargetEnd - _scintilla.TargetStart));
                    _scintilla.ScrollCaret();
                }
                else
                {
                    MessageBox.Show("No encontrado", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("No encontrado", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FindPrevious()
        {
            if (_scintilla == null) return;
            string pattern = txtFind.Text;
            if (string.IsNullOrEmpty(pattern)) return;

            // Simpler approach para buscar hacia atrás: buscar en la parte previa y tomar la última ocurrencia
            int currentStart = _scintilla.SelectionStart;
            string before = _scintilla.Text.Substring(0, currentStart);

            if (chkRegex.Checked)
            {
                try
                {
                    var options = chkMatchCase.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
                    var matches = Regex.Matches(before, pattern, options);
                    if (matches.Count > 0)
                    {
                        var m = matches[matches.Count - 1];
                        _scintilla.SetSelection(m.Index, m.Index + m.Length);
                        _scintilla.ScrollCaret();
                        return;
                    }
                }
                catch (ArgumentException ex)
                {
                    MessageBox.Show("Regex inválido: " + ex.Message);
                    return;
                }
            }
            else
            {
                var comparison = chkMatchCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                int idx = before.LastIndexOf(pattern, comparison);
                if (idx != -1)
                {
                    _scintilla.SetSelection(idx, idx + pattern.Length);
                    _scintilla.ScrollCaret();
                    return;
                }
            }

            if (chkWrap.Checked)
            {
                // buscar desde el final hacia currentStart (buscamos en toda la cadena excepto la parte ya inspeccionada)
                string after = _scintilla.Text.Substring(currentStart); // parte después de cursor
                                                                        // para wrap backward: buscamos la última ocurrencia en toda la doc antes del cursor (ya lo hicimos), si no, buscamos desde end backwards:
                string all = _scintilla.Text;
                if (chkRegex.Checked)
                {
                    try
                    {
                        var options = chkMatchCase.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
                        var matches = Regex.Matches(all, pattern, options);
                        if (matches.Count > 0)
                        {
                            var m = matches[matches.Count - 1];
                            _scintilla.SetSelection(m.Index, m.Index + m.Length);
                            _scintilla.ScrollCaret();
                            return;
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show("Regex inválido: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    var comparison = chkMatchCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                    int idx = all.LastIndexOf(pattern, comparison);
                    if (idx != -1)
                    {
                        _scintilla.SetSelection(idx, idx + pattern.Length);
                        _scintilla.ScrollCaret();
                        return;
                    }
                }
            }

            MessageBox.Show("No encontrado", "Find", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}
