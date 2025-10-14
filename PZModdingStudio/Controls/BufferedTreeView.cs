using System;
using System.Reflection;
using System.Windows.Forms;

public class BufferedTreeView : TreeView
{
    public BufferedTreeView()
    {
        // Activar estilos de pintura optimizada
        this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer
                      | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint
                      | System.Windows.Forms.ControlStyles.UserPaint, true);
        this.UpdateStyles();

        // Forzar la propiedad protegida DoubleBuffered mediante reflexión (seguro)
        var prop = typeof(TreeView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (prop != null)
            prop.SetValue(this, true, null);
    }

    // Opcional: ignorar erase background puede ayudar en algunos casos
    protected override void OnNotifyMessage(System.Windows.Forms.Message m)
    {
        // WM_ERASEBKGND = 0x0014
        if (m.Msg != 0x0014)
            base.OnNotifyMessage(m);
    }
}
