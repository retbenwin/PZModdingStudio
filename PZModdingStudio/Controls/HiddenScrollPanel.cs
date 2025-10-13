using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class HiddenScrollPanel : Panel
{
    // WinAPI
    [DllImport("user32.dll")]
    private static extern bool ShowScrollBar(IntPtr hWnd, int wBar, bool bShow);

    private const int SB_HORZ = 0;
    private const int SB_VERT = 1;
    private const int SB_BOTH = 3;

    // Opciones de CreateParams para intentar remover estilos nativos de scroll
    protected override CreateParams CreateParams
    {
        get
        {
            const int WS_HSCROLL = 0x00100000;
            const int WS_VSCROLL = 0x00200000;

            var cp = base.CreateParams;
            cp.Style &= ~WS_HSCROLL;
            cp.Style &= ~WS_VSCROLL;
            return cp;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        // Forzar ocultado después de creado el handle
        HideScrollbars();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        HideScrollbars();
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        HideScrollbars();
    }

    // Manejar mensajes de cambio de estilo/tamaño por si Windows vuelve a mostrar las barras
    protected override void WndProc(ref Message m)
    {
        const int WM_STYLECHANGED = 0x007D;
        const int WM_WINDOWPOSCHANGED = 0x0047;
        const int WM_SIZE = 0x0005;

        base.WndProc(ref m);

        if (m.Msg == WM_STYLECHANGED || m.Msg == WM_WINDOWPOSCHANGED || m.Msg == WM_SIZE)
        {
            HideScrollbars();
        }
    }

    private void HideScrollbars()
    {
        if (this.IsHandleCreated)
        {
            // Oculta ambas barras
            ShowScrollBar(this.Handle, SB_BOTH, false);
        }
    }

    // Si quieres que la rueda del ratón funcione sin barras visibles:
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        base.OnMouseWheel(e);
        if (this.AutoScroll)
        {
            // Comportamiento de scroll vertical estándar
            var newY = Math.Max(0, this.VerticalScroll.Value - e.Delta);
            // Ajustar dentro de límites
            newY = Math.Min(this.VerticalScroll.Maximum, newY);
            this.AutoScrollPosition = new System.Drawing.Point(this.AutoScrollPosition.X, newY);
            HideScrollbars(); // asegurar que siguen ocultas
        }
    }
}

