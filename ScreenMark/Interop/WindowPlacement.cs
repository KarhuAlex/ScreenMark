using System.Drawing;
using System.Windows.Interop;
using Window = System.Windows.Window;

namespace ScreenMark.Interop;

internal static class WindowPlacement
{
    // Windows only rescales a window for the new monitor's DPI after it has been moved there,
    // so the exact pixel bounds are applied a second time once the move has settled.
    public static void SetPixelBounds(Window window, Rectangle bounds)
    {
        var handle = new WindowInteropHelper(window).EnsureHandle();
        SetBounds(handle, bounds);
        SetBounds(handle, bounds);
    }

    public static void MoveToPixel(Window window, Point location)
    {
        var handle = new WindowInteropHelper(window).EnsureHandle();
        _ = NativeMethods.SetWindowPos(
            handle,
            nint.Zero,
            location.X,
            location.Y,
            0,
            0,
            NativeMethods.SwpNoSize | NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);
    }

    public static void HideFromTaskSwitcher(Window window)
    {
        var handle = new WindowInteropHelper(window).EnsureHandle();
        var style = NativeMethods.GetWindowLong(handle, NativeMethods.GwlExStyle);
        _ = NativeMethods.SetWindowLong(handle, NativeMethods.GwlExStyle, style | NativeMethods.WsExToolWindow);
    }

    public static bool ExcludeFromCapture(Window window)
    {
        var handle = new WindowInteropHelper(window).EnsureHandle();
        return NativeMethods.SetWindowDisplayAffinity(handle, NativeMethods.WdaExcludeFromCapture);
    }

    private static void SetBounds(nint handle, Rectangle bounds) =>
        NativeMethods.SetWindowPos(
            handle,
            nint.Zero,
            bounds.X,
            bounds.Y,
            bounds.Width,
            bounds.Height,
            NativeMethods.SwpNoZOrder | NativeMethods.SwpNoActivate);
}
