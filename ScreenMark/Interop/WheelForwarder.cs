using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ScreenMark.Interop;

internal sealed class WheelForwarder
{
    private static readonly TimeSpan ClickThroughDuration = TimeSpan.FromMilliseconds(60);

    private readonly Window overlay;
    private readonly DispatcherTimer restoreTimer;
    private bool isClickThrough;

    public WheelForwarder(Window overlay)
    {
        this.overlay = overlay;
        restoreTimer = new DispatcherTimer { Interval = ClickThroughDuration };
        restoreTimer.Tick += (_, _) => Stop();
    }

    public void Forward(int delta)
    {
        SetClickThrough(enabled: true);

        Input[] inputs = [Input.Wheel(delta)];
        _ = NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Input>());

        restoreTimer.Stop();
        restoreTimer.Start();
    }

    public void Stop()
    {
        restoreTimer.Stop();
        SetClickThrough(enabled: false);
    }

    private void SetClickThrough(bool enabled)
    {
        if (isClickThrough == enabled)
        {
            return;
        }

        var handle = new WindowInteropHelper(overlay).Handle;
        var style = NativeMethods.GetWindowLong(handle, NativeMethods.GwlExStyle);
        var updated = enabled ? style | NativeMethods.WsExTransparent : style & ~NativeMethods.WsExTransparent;
        _ = NativeMethods.SetWindowLong(handle, NativeMethods.GwlExStyle, updated);
        isClickThrough = enabled;
    }
}
