using System.Windows.Input;
using System.Windows.Interop;
using ScreenMark.Interop;

namespace ScreenMark;

internal sealed class HotkeyService : IDisposable
{
    private const int hotkeyId = 1;
    private static readonly nint messageOnlyParent = new(-3);

    private readonly ModifierKeys modifiers;
    private readonly Key key;
    private readonly HwndSource messageWindow;
    private bool isRegistered;

    public HotkeyService(ModifierKeys modifiers, Key key)
    {
        this.modifiers = modifiers;
        this.key = key;

        messageWindow = new HwndSource(new HwndSourceParameters("ScreenMark.Hotkey") { ParentWindow = messageOnlyParent });
        messageWindow.AddHook(OnMessage);
    }

    public event EventHandler? Pressed;

    // Fails when another application already owns the key combination.
    public bool TryRegister()
    {
        isRegistered = NativeMethods.RegisterHotKey(
            messageWindow.Handle,
            hotkeyId,
            (uint)modifiers | NativeMethods.ModNoRepeat,
            (uint)KeyInterop.VirtualKeyFromKey(key));

        return isRegistered;
    }

    public void Dispose()
    {
        if (isRegistered)
        {
            _ = NativeMethods.UnregisterHotKey(messageWindow.Handle, hotkeyId);
        }

        messageWindow.RemoveHook(OnMessage);
        messageWindow.Dispose();
    }

    private nint OnMessage(nint hwnd, int message, nint wParam, nint lParam, ref bool handled)
    {
        if (message == NativeMethods.WmHotkey && wParam == hotkeyId)
        {
            Pressed?.Invoke(this, EventArgs.Empty);
            handled = true;
        }

        return nint.Zero;
    }
}
