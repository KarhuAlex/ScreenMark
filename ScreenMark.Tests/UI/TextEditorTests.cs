using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ScreenMark.Drawing;
using ScreenMark.UI;

namespace ScreenMark.Tests.UI;

public class TextEditorTests
{
    private const int CanvasWidth = 240;
    private const int CanvasHeight = 80;

    [Fact]
    public void Commit_RaisesCompletedWithTrimmedTextAtTheClickedPosition() => StaThread.Run(() =>
    {
        var (editor, surface) = CreateEditor();
        Annotation? completed = null;
        editor.Completed += (_, annotation) => completed = annotation;

        editor.Begin(new Point(30, 20), Palette.Blue);
        TypeInto(surface, "Hello  \n");
        editor.Commit();

        var block = Assert.IsType<TextBlock>(completed?.Visual);
        Assert.Equal("Hello", block.Text);
        Assert.Equal(30, Canvas.GetLeft(block));
        Assert.Equal(20, Canvas.GetTop(block));
        Assert.Equal(Palette.Blue, ((SolidColorBrush)block.Foreground).Color);
    });

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\n\n")]
    public void Commit_IgnoresBlankText(string text) => StaThread.Run(() =>
    {
        var (editor, surface) = CreateEditor();
        var completedCount = 0;
        editor.Completed += (_, _) => completedCount++;

        editor.Begin(new Point(0, 0), Palette.Red);
        TypeInto(surface, text);
        editor.Commit();

        Assert.Equal(0, completedCount);
        Assert.False(editor.IsActive);
        Assert.Empty(surface.Children);
    });

    [Fact]
    public void Cancel_RemovesTheEditorWithoutCompleting() => StaThread.Run(() =>
    {
        var (editor, surface) = CreateEditor();
        var completedCount = 0;
        editor.Completed += (_, _) => completedCount++;

        editor.Begin(new Point(0, 0), Palette.Red);
        TypeInto(surface, "discarded");
        editor.Cancel();

        Assert.Equal(0, completedCount);
        Assert.False(editor.IsActive);
        Assert.Empty(surface.Children);
    });

    [Fact]
    public void Commit_IsANoOpWhenNothingIsBeingEdited() => StaThread.Run(() =>
    {
        var (editor, _) = CreateEditor();
        var completedCount = 0;
        editor.Completed += (_, _) => completedCount++;

        editor.Commit();
        editor.Cancel();

        Assert.Equal(0, completedCount);
        Assert.False(editor.IsActive);
    });

    [Fact]
    public void PlacedText_AppearsWhereItWasTyped() => StaThread.Run(() =>
    {
        var (editor, surface) = CreateEditor();
        Annotation? completed = null;
        editor.Completed += (_, annotation) => completed = annotation;

        editor.Begin(new Point(20, 10), Palette.Red);
        TypeInto(surface, "Hello");
        var typedBounds = InkBounds(surface);
        editor.Commit();

        var placed = new Canvas { Background = Brushes.White };
        placed.Children.Add(completed!.Visual);

        Assert.Equal(typedBounds, InkBounds(placed));
    });

    private static (TextEditor Editor, Canvas Surface) CreateEditor()
    {
        var surface = new Canvas { Width = CanvasWidth, Height = CanvasHeight, Background = Brushes.White };
        return (new TextEditor(surface), surface);
    }

    private static void TypeInto(Canvas surface, string text) => ((TextBox)surface.Children[0]).Text = text;

    private static Int32Rect InkBounds(Canvas surface)
    {
        surface.Measure(new Size(CanvasWidth, CanvasHeight));
        surface.Arrange(new Rect(0, 0, CanvasWidth, CanvasHeight));
        surface.UpdateLayout();

        var bitmap = new RenderTargetBitmap(CanvasWidth, CanvasHeight, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(surface);

        var stride = CanvasWidth * 4;
        var pixels = new byte[stride * CanvasHeight];
        bitmap.CopyPixels(pixels, stride, 0);

        int left = CanvasWidth, top = CanvasHeight, right = -1, bottom = -1;
        for (var y = 0; y < CanvasHeight; y++)
        {
            for (var x = 0; x < CanvasWidth; x++)
            {
                var offset = (y * stride) + (x * 4);
                if (pixels[offset + 1] < 128)
                {
                    left = Math.Min(left, x);
                    top = Math.Min(top, y);
                    right = Math.Max(right, x);
                    bottom = Math.Max(bottom, y);
                }
            }
        }

        return new Int32Rect(left, top, right - left + 1, bottom - top + 1);
    }
}
