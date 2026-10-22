using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ScreenMark.Drawing;

internal sealed class Annotation
{
    private const double minimumExtent = 3;

    private readonly DrawingTool tool;
    private readonly Point origin;
    private Rect extent;

    private Annotation(DrawingTool tool, Point origin, Shape visual)
    {
        this.tool = tool;
        this.origin = origin;
        extent = new Rect(origin, origin);
        Visual = visual;
    }

    public Shape Visual { get; }

    public bool IsSignificant => extent.Width >= minimumExtent || extent.Height >= minimumExtent;

    public static Annotation Begin(DrawingTool tool, Point origin, Color color, double thickness)
    {
        var stroke = new SolidColorBrush(color);
        stroke.Freeze();

        Shape visual = tool switch
        {
            DrawingTool.Rectangle => new Rectangle(),
            DrawingTool.Ellipse => new Ellipse(),
            DrawingTool.Arrow => new Path(),
            DrawingTool.Freehand => new Polyline { Points = [origin] },
            _ => throw new ArgumentOutOfRangeException(nameof(tool)),
        };

        visual.Stroke = stroke;
        visual.StrokeThickness = thickness;
        visual.StrokeStartLineCap = PenLineCap.Round;
        visual.StrokeEndLineCap = PenLineCap.Round;
        visual.StrokeLineJoin = tool == DrawingTool.Rectangle ? PenLineJoin.Miter : PenLineJoin.Round;

        return new Annotation(tool, origin, visual);
    }

    public void Extend(Point point)
    {
        extent.Union(point);

        switch (tool)
        {
            case DrawingTool.Rectangle or DrawingTool.Ellipse:
                var bounds = new Rect(origin, point);
                Canvas.SetLeft(Visual, bounds.X);
                Canvas.SetTop(Visual, bounds.Y);
                Visual.Width = bounds.Width;
                Visual.Height = bounds.Height;
                break;
            case DrawingTool.Arrow:
                ((Path)Visual).Data = ArrowGeometry.Create(origin, point);
                break;
            case DrawingTool.Freehand:
                ((Polyline)Visual).Points.Add(point);
                break;
        }
    }
}
