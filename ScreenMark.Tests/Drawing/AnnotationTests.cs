using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using ScreenMark.Drawing;

namespace ScreenMark.Tests.Drawing;

public class AnnotationTests
{
    [Fact]
    public void Rectangle_NormalizesBoundsForEveryDragDirection() => AssertBoundsAreNormalized(DrawingTool.Rectangle);

    [Fact]
    public void Ellipse_NormalizesBoundsForEveryDragDirection() => AssertBoundsAreNormalized(DrawingTool.Ellipse);

    [Fact]
    public void Begin_AppliesColorAndThickness() => StaThread.Run(() =>
    {
        var shape = (Shape)Annotation.Begin(DrawingTool.Rectangle, new Point(0, 0), Palette.Blue, 5).Visual;

        Assert.Equal(Palette.Blue, ((SolidColorBrush)shape.Stroke).Color);
        Assert.Equal(5, shape.StrokeThickness);
    });

    [Fact]
    public void Freehand_RecordsEveryPointFromOriginOnwards() => StaThread.Run(() =>
    {
        var annotation = Annotation.Begin(DrawingTool.Freehand, new Point(1, 1), Palette.Green, 3);
        annotation.Extend(new Point(5, 3));
        annotation.Extend(new Point(9, 2));

        Assert.Equal([new Point(1, 1), new Point(5, 3), new Point(9, 2)], ((Polyline)annotation.Visual).Points);
    });

    [Fact]
    public void Arrow_UpdatesGeometryWhileDragging() => StaThread.Run(() =>
    {
        var annotation = Annotation.Begin(DrawingTool.Arrow, new Point(0, 0), Palette.Red, 3);
        var path = (Path)annotation.Visual;

        annotation.Extend(new Point(100, 0));

        Assert.NotNull(path.Data);
        Assert.False(path.Data.IsEmpty());
    });

    [Theory]
    [InlineData(1, 1, false)]
    [InlineData(2.9, 2.9, false)]
    [InlineData(3, 0, true)]
    [InlineData(0, -40, true)]
    public void IsSignificant_RequiresMinimumExtentInEitherAxis(double dx, double dy, bool expected) => StaThread.Run(() =>
    {
        var annotation = Annotation.Begin(DrawingTool.Rectangle, new Point(50, 50), Palette.Red, 3);
        annotation.Extend(new Point(50 + dx, 50 + dy));

        Assert.Equal(expected, annotation.IsSignificant);
    });

    [Fact]
    public void IsSignificant_CountsFreehandStrokeThatReturnsToItsOrigin() => StaThread.Run(() =>
    {
        var annotation = Annotation.Begin(DrawingTool.Freehand, new Point(10, 10), Palette.Red, 3);
        annotation.Extend(new Point(60, 10));
        annotation.Extend(new Point(60, 60));
        annotation.Extend(new Point(10, 10));

        Assert.True(annotation.IsSignificant);
    });

    [Fact]
    public void Begin_RejectsTextTool() => StaThread.Run(() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => Annotation.Begin(DrawingTool.Text, new Point(0, 0), Palette.Red, 3)));

    [Fact]
    public void CreateText_UsesGivenTextColorAndPosition() => StaThread.Run(() =>
    {
        var annotation = Annotation.CreateText(new Point(40, 25), "Label", Palette.Green);
        var block = (TextBlock)annotation.Visual;

        Assert.Equal("Label", block.Text);
        Assert.Equal(Palette.Green, ((SolidColorBrush)block.Foreground).Color);
        Assert.Equal(40, Canvas.GetLeft(block));
        Assert.Equal(25, Canvas.GetTop(block));
    });

    [Fact]
    public void CreateText_AppliesSharedTextStyle() => StaThread.Run(() =>
    {
        var block = (TextBlock)Annotation.CreateText(new Point(0, 0), "Label", Palette.Red).Visual;

        Assert.Equal(TextStyle.Family, block.FontFamily);
        Assert.Equal(TextStyle.Size, block.FontSize);
        Assert.Equal(TextStyle.Weight, block.FontWeight);
        Assert.Equal(new Thickness(TextStyle.Inset), block.Padding);
    });

    [Fact]
    public void IsSignificant_IsTrueForTextWithoutDragging() => StaThread.Run(() =>
        Assert.True(Annotation.CreateText(new Point(5, 5), "Label", Palette.Red).IsSignificant));

    private static void AssertBoundsAreNormalized(DrawingTool tool) => StaThread.Run(() =>
    {
        var center = new Point(100, 100);
        Point[] corners = [new(160, 130), new(40, 130), new(160, 70), new(40, 70)];

        foreach (var corner in corners)
        {
            var annotation = Annotation.Begin(tool, center, Palette.Red, 3);
            annotation.Extend(corner);

            Assert.Equal(Math.Min(center.X, corner.X), Canvas.GetLeft(annotation.Visual));
            Assert.Equal(Math.Min(center.Y, corner.Y), Canvas.GetTop(annotation.Visual));
            Assert.Equal(Math.Abs(corner.X - center.X), annotation.Visual.Width);
            Assert.Equal(Math.Abs(corner.Y - center.Y), annotation.Visual.Height);
        }
    });
}
