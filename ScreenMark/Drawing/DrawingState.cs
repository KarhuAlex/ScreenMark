using System.Windows.Media;

namespace ScreenMark.Drawing;

internal sealed class DrawingState
{
    public const double StrokeThickness = 3;

    public DrawingTool Tool { get; set; } = DrawingTool.Rectangle;

    public Color Color { get; set; } = Palette.Red;
}
