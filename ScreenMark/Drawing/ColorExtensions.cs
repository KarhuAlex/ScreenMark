using System.Windows.Media;

namespace ScreenMark.Drawing;

internal static class ColorExtensions
{
    public static SolidColorBrush ToFrozenBrush(this Color color)
    {
        var brush = new SolidColorBrush(color);
        brush.Freeze();
        return brush;
    }
}
