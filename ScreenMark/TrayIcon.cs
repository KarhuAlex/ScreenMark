using System.Drawing;
using System.Windows.Forms;

namespace ScreenMark;

internal sealed class TrayIcon : IDisposable
{
    private readonly NotifyIcon notifyIcon;
    private readonly ContextMenuStrip menu = new();

    public TrayIcon(Action changeHotkey, Action exit)
    {
        _ = menu.Items.Add("Change hotkey…", null, (_, _) => changeHotkey());
        _ = menu.Items.Add("Exit", null, (_, _) => exit());

        using var stream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Assets/ScreenMark.ico")).Stream;
        notifyIcon = new NotifyIcon
        {
            Icon = new Icon(stream, SystemInformation.SmallIconSize),
            ContextMenuStrip = menu,
            Visible = true,
        };
    }

    public void SetText(string text) => notifyIcon.Text = text;

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Icon?.Dispose();
        notifyIcon.Dispose();
        menu.Dispose();
    }
}
