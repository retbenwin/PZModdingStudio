using PZModdingStudio.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class CommandsManager : IDisposable
{
    private FrmBase form;
    private List<Keys> keysPressed;

    public enum CommandType { Save, SaveAll, Find, Null }

    public CommandsManager(FrmBase form)
    {
        this.form = form;
        this.keysPressed = new List<Keys>();

        // necesario para que el formulario reciba KeyDown antes que los controles
        this.form.KeyPreview = true;

        SubscribeEvents(form);
    }

    public void Dispose()
    {
        UnsubscribeEvents(form);
    }

    private void SubscribeEvents(FrmBase frm)
    {
        frm.KeyDown += KeyDownHandler;
        frm.KeyUp += KeyUpHandler;
        frm.Deactivate += Form_Deactivate;
        frm.Activated += Form_Activated;
    }

    private void UnsubscribeEvents(FrmBase frm)
    {
        frm.KeyDown -= KeyDownHandler;
        frm.KeyUp -= KeyUpHandler;
        frm.Deactivate -= Form_Deactivate;
        frm.Activated -= Form_Activated;
    }

    public bool IsSavePressed() =>
        keysPressed.Contains(Keys.ControlKey) && keysPressed.Contains(Keys.S);

    public bool IsSaveAllPressed() =>
        keysPressed.Contains(Keys.ControlKey) && keysPressed.Contains(Keys.ShiftKey) && keysPressed.Contains(Keys.A);

    public bool IsFindPressed() =>
        keysPressed.Contains(Keys.ControlKey) && keysPressed.Contains(Keys.F);

    public void ResetKeys() => this.keysPressed.Clear();

    private CommandType TriggerCommandPressed()
    {
        CommandType ct;
        switch (true)
        {
            case bool _ when IsSaveAllPressed():
                ct = CommandType.SaveAll; break;
            case bool _ when IsSavePressed():
                ct = CommandType.Save; break;
            case bool _ when IsFindPressed():
                ct = CommandType.Find; break;
            default:
                ct = CommandType.Null; break;
        }
        return ct;
    }

    private void KeyDownHandler(object sender, KeyEventArgs e)
    {
        if (!this.keysPressed.Contains(e.KeyCode))
        {
            this.keysPressed.Add(e.KeyCode);
        }

        // checamos si hay un comando
        var cmd = TriggerCommandPressed();
        if (cmd != CommandType.Null)
        {
            // lanzamos evento
            OnCommandPressed?.Invoke(this, new CommandPressedArgs(cmd));

            // IMPORTANTE: suprimir la tecla para que Scintilla no inserte caracteres raros
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void KeyUpHandler(object sender, KeyEventArgs e)
    {
        if (this.keysPressed.Contains(e.KeyCode))
        {
            this.keysPressed.Remove(e.KeyCode);
        }
    }

    private void Form_Deactivate(object sender, EventArgs e)
    {
        // Al perder foco (por ejemplo aparece un MessageBox), limpiar para evitar "pegado"
        ResetKeys();
    }

    private void Form_Activated(object sender, EventArgs e)
    {
        // Reconstruir modificadores reales (por si el usuario mantiene Ctrl/Shift al volver)
        ResetKeys();
        if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            keysPressed.Add(Keys.ControlKey);
        if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            keysPressed.Add(Keys.ShiftKey);
    }

    public event EventHandler<CommandPressedArgs> OnCommandPressed;

    public class CommandPressedArgs : EventArgs
    {
        public CommandType commandType { get; }
        public CommandPressedArgs(CommandType commandType) { this.commandType = commandType; }
    }
}
