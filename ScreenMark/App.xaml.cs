using System.Windows;
using System.Windows.Input;

namespace ScreenMark;

internal sealed partial class App : Application, IDisposable
{
    private const string InstanceMutexName = @"Local\ScreenMark";

    private readonly OverlaySession session = new();
    private HotkeyService? hotkey;
    private TrayIcon? trayIcon;
    private Mutex? instanceMutex;

    public void Dispose()
    {
        trayIcon?.Dispose();
        hotkey?.Dispose();
        session.Dispose();
        instanceMutex?.ReleaseMutex();
        instanceMutex?.Dispose();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        instanceMutex = new Mutex(initiallyOwned: true, InstanceMutexName, out var isFirstInstance);
        if (!isFirstInstance)
        {
            instanceMutex.Dispose();
            instanceMutex = null;
            Shutdown();
            return;
        }

        hotkey = new HotkeyService(ModifierKeys.Control, Key.D0);
        if (!hotkey.TryRegister())
        {
            MessageBox.Show(
                "Ctrl+0 is already in use by another application.",
                "ScreenMark",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        hotkey.Pressed += (_, _) => session.Toggle();
        trayIcon = new TrayIcon("ScreenMark (Ctrl+0)", Shutdown);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Dispose();
        base.OnExit(e);
    }
}
