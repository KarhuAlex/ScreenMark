using System.Drawing;
using System.Windows.Forms;

namespace ScreenMark;

internal sealed class TrayIcon : IDisposable
{
    private readonly NotifyIcon notifyIcon;
    private readonly ContextMenuStrip menu = new();

    public TrayIcon(string text, Action exit)
    {
        _ = menu.Items.Add("Exit", null, (_, _) => exit());

        using var stream = System.Windows.Application.GetResourceStream(new Uri("pack://application:,,,/Assets/ScreenMark.ico")).Stream;
        notifyIcon = new NotifyIcon
        {
            Icon = new Icon(stream, SystemInformation.SmallIconSize),
            Text = text,
            ContextMenuStrip = menu,
            Visible = true,
        };
    }

    public void Dispose()
    {
        notifyIcon.Visible = false;
        notifyIcon.Icon?.Dispose();
        notifyIcon.Dispose();
        menu.Dispose();
    }
}
