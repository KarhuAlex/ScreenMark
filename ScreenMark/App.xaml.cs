using System.Windows;
using ScreenMark.UI;

namespace ScreenMark;

internal sealed partial class App : Application, IDisposable
{
    private const string InstanceMutexName = @"Local\ScreenMark";

    private readonly OverlaySession session = new();
    private readonly HotkeyService hotkeyService = new();
    private readonly SettingsStore settings = new(SettingsStore.DefaultPath);
    private TrayIcon? trayIcon;
    private Mutex? instanceMutex;
    private bool isChoosingHotkey;

    public void Dispose()
    {
        trayIcon?.Dispose();
        hotkeyService.Dispose();
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

        var tray = new TrayIcon(() => ChooseHotkey(message: null), Shutdown);
        trayIcon = tray;
        hotkeyService.Changed += (_, hotkey) => tray.SetText($"ScreenMark ({hotkey})");
        hotkeyService.Pressed += OnHotkeyPressed;

        var configured = settings.LoadHotkey();
        if (!hotkeyService.TryRegister(configured)
            && !ChooseHotkey($"{configured} is already in use by another application."))
        {
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Dispose();
        base.OnExit(e);
    }

    private void OnHotkeyPressed(object? sender, EventArgs e)
    {
        if (!isChoosingHotkey)
        {
            session.Toggle();
        }
    }

    private bool ChooseHotkey(string? message)
    {
        isChoosingHotkey = true;
        try
        {
            return new HotkeyDialog(TryApplyHotkey, message).ShowDialog() == true;
        }
        finally
        {
            isChoosingHotkey = false;
        }
    }

    private bool TryApplyHotkey(Hotkey hotkey)
    {
        if (!hotkeyService.TryRegister(hotkey))
        {
            return false;
        }

        settings.SaveHotkey(hotkey);
        return true;
    }
}
