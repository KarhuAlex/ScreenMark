using System.Windows;
using System.Windows.Media;

namespace ScreenMark.Drawing;

internal static class ArrowGeometry
{
    private const double HeadLength = 16;
    private const double HeadHalfAngleDegrees = 25;

    public static Geometry Create(Point start, Point end)
    {
        if (start == end)
        {
            return Geometry.Empty;
        }

        var (left, right) = GetHeadWingTips(start, end);
        var geometry = new StreamGeometry();

        using (var context = geometry.Open())
        {
            context.BeginFigure(start, isFilled: false, isClosed: false);
            context.LineTo(end, isStroked: true, isSmoothJoin: false);
            context.BeginFigure(left, isFilled: false, isClosed: false);
            context.LineTo(end, isStroked: true, isSmoothJoin: true);
            context.LineTo(right, isStroked: true, isSmoothJoin: true);
        }

        geometry.Freeze();
        return geometry;
    }

    public static (Point Left, Point Right) GetHeadWingTips(Point start, Point end)
    {
        var backward = start - end;
        var headLength = Math.Min(HeadLength, backward.Length / 2);
        backward.Normalize();

        return (
            end + (Rotate(backward, HeadHalfAngleDegrees) * headLength),
            end + (Rotate(backward, -HeadHalfAngleDegrees) * headLength));
    }

    private static Vector Rotate(Vector vector, double degrees)
    {
        var matrix = new Matrix();
        matrix.Rotate(degrees);
        return vector * matrix;
    }
}
