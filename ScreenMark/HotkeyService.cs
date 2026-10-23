using System.Windows.Input;
using System.Windows.Interop;
using ScreenMark.Interop;

namespace ScreenMark;

internal sealed class HotkeyService : IDisposable
{
    private const int HotkeyId = 1;
    private static readonly nint MessageOnlyParent = new(-3);

    private readonly HwndSource messageWindow;
    private Hotkey? registered;

    public HotkeyService()
    {
        messageWindow = new HwndSource(new HwndSourceParameters("ScreenMark.Hotkey") { ParentWindow = MessageOnlyParent });
        messageWindow.AddHook(OnMessage);
    }

    public event EventHandler? Pressed;

    public event EventHandler<Hotkey>? Changed;

    public bool TryRegister(Hotkey hotkey)
    {
        var previous = registered;
        Unregister();

        if (Register(hotkey))
        {
            Changed?.Invoke(this, hotkey);
            return true;
        }

        if (previous is { } restored)
        {
            _ = Register(restored);
        }

        return false;
    }

    public void Dispose()
    {
        Unregister();
        messageWindow.RemoveHook(OnMessage);
        messageWindow.Dispose();
    }

    private bool Register(Hotkey hotkey)
    {
        var succeeded = NativeMethods.RegisterHotKey(
            messageWindow.Handle,
            HotkeyId,
            (uint)hotkey.Modifiers | NativeMethods.ModNoRepeat,
            (uint)KeyInterop.VirtualKeyFromKey(hotkey.Key));

        registered = succeeded ? hotkey : null;
        return succeeded;
    }

    private void Unregister()
    {
        if (registered is not null)
        {
            _ = NativeMethods.UnregisterHotKey(messageWindow.Handle, HotkeyId);
            registered = null;
        }
    }

    private nint OnMessage(nint hwnd, int message, nint wParam, nint lParam, ref bool handled)
    {
        if (message == NativeMethods.WmHotkey && wParam == HotkeyId)
        {
            Pressed?.Invoke(this, EventArgs.Empty);
            handled = true;
        }

        return nint.Zero;
    }
}
