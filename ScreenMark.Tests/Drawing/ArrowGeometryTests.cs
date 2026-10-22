using System.Windows;
using System.Windows.Media;
using ScreenMark.Drawing;

namespace ScreenMark.Tests.Drawing;

public class ArrowGeometryTests
{
    private const double tolerance = 1e-9;

    [Theory]
    [InlineData(0, 0, 100, 0)]
    [InlineData(100, 100, 0, 0)]
    [InlineData(20, 80, 120, 10)]
    public void HeadWingTips_AreSymmetricAboutTheShaft(double x1, double y1, double x2, double y2)
    {
        var start = new Point(x1, y1);
        var end = new Point(x2, y2);

        var (left, right) = ArrowGeometry.GetHeadWingTips(start, end);

        Assert.Equal((end - left).Length, (end - right).Length, tolerance);
        var midpoint = new Point((left.X + right.X) / 2, (left.Y + right.Y) / 2);
        Assert.Equal(0, Vector.CrossProduct(end - start, midpoint - end), 1e-6);
    }

    [Fact]
    public void HeadWingTips_PointBackTowardsTheStart()
    {
        var (left, right) = ArrowGeometry.GetHeadWingTips(new Point(0, 0), new Point(100, 0));

        Assert.True(left.X < 100);
        Assert.True(right.X < 100);
        Assert.True(left.Y * right.Y < 0);
    }

    [Fact]
    public void HeadWingTips_ShrinkWithShortArrows()
    {
        var (left, _) = ArrowGeometry.GetHeadWingTips(new Point(0, 0), new Point(10, 0));

        Assert.Equal(5, (new Point(10, 0) - left).Length, tolerance);
    }

    [Fact]
    public void Create_ReturnsEmptyGeometryForZeroLengthArrow() => Assert.Same(Geometry.Empty, ArrowGeometry.Create(new Point(5, 5), new Point(5, 5)));

    [Fact]
    public void Create_CoversShaftAndHead()
    {
        var bounds = ArrowGeometry.Create(new Point(0, 0), new Point(100, 0)).Bounds;

        Assert.Equal(0, bounds.Left, tolerance);
        Assert.Equal(100, bounds.Right, tolerance);
        Assert.True(bounds.Height > 0);
    }
}
