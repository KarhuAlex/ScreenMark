using System.Windows;
using System.Windows.Input;
using ScreenMark.Drawing;
using ScreenMark.Interop;

namespace ScreenMark.UI;

internal sealed partial class OverlayWindow : Window
{
    private readonly DrawingState state;
    private readonly System.Drawing.Rectangle screenBounds;
    private Annotation? current;

    public OverlayWindow(DrawingState state, System.Drawing.Rectangle screenBounds)
    {
        this.state = state;
        this.screenBounds = screenBounds;
        InitializeComponent();
    }

    public event EventHandler<Annotation>? AnnotationCompleted;

    public void Remove(Annotation annotation) => Surface.Children.Remove(annotation.Visual);

    public void Clear()
    {
        CancelCurrent();
        Surface.Children.Clear();
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        WindowPlacement.HideFromTaskSwitcher(this);
        WindowPlacement.SetPixelBounds(this, screenBounds);
    }

    private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var origin = e.GetPosition(Surface);
        current = Annotation.Begin(state.Tool, origin, state.Color, DrawingState.StrokeThickness);
        Surface.Children.Add(current.Visual);
        Surface.CaptureMouse();
    }

    private void OnMouseMove(object sender, MouseEventArgs e) => current?.Extend(e.GetPosition(Surface));

    private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (current is not { } annotation)
        {
            return;
        }

        current = null;
        Surface.ReleaseMouseCapture();
        annotation.Extend(e.GetPosition(Surface));

        if (annotation.IsSignificant)
        {
            AnnotationCompleted?.Invoke(this, annotation);
        }
        else
        {
            Remove(annotation);
        }
    }

    private void OnLostMouseCapture(object sender, MouseEventArgs e) => CancelCurrent();

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && current is not null)
        {
            CancelCurrent();
            e.Handled = true;
        }
    }

    private void CancelCurrent()
    {
        if (current is not { } annotation)
        {
            return;
        }

        current = null;
        Remove(annotation);
        Surface.ReleaseMouseCapture();
    }
}
