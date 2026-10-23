using System.Windows;
using ScreenMark.Interop;

namespace ScreenMark.Tests.Interop;

public class WindowPlacementTests
{
    [Fact]
    public void ExcludeFromCapture_IsAcceptedForTransparentWindows() => StaThread.Run(() =>
    {
        var window = new Window { WindowStyle = WindowStyle.None, AllowsTransparency = true, ShowInTaskbar = false };

        try
        {
            Assert.True(WindowPlacement.ExcludeFromCapture(window));
        }
        finally
        {
            window.Close();
        }
    });
}
