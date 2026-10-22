using System.Windows.Forms;
using ScreenMark.Drawing;
using ScreenMark.UI;

namespace ScreenMark;

internal sealed class OverlaySession : IDisposable
{
    private readonly List<OverlayWindow> overlays = [];
    private readonly Stack<(OverlayWindow Overlay, Annotation Annotation)> history = new();
    private ToolbarWindow? toolbar;

    public bool IsActive => toolbar is not null;

    public void Toggle()
    {
        if (IsActive)
        {
            Dismiss();
        }
        else
        {
            Start();
        }
    }

    public void Dispose() => Dismiss();

    private void Start()
    {
        var state = new DrawingState();
        OverlayWindow? primaryOverlay = null;
        var primaryBounds = System.Drawing.Rectangle.Empty;

        foreach (var screen in Screen.AllScreens)
        {
            var overlay = new OverlayWindow(state, screen.Bounds);
            overlay.AnnotationCompleted += OnAnnotationCompleted;
            overlay.Show();
            overlays.Add(overlay);

            if (screen.Primary)
            {
                primaryOverlay = overlay;
                primaryBounds = screen.Bounds;
            }
        }

        toolbar = new ToolbarWindow(state, primaryBounds) { Owner = primaryOverlay };
        toolbar.UndoRequested += (_, _) => Undo();
        toolbar.ClearRequested += (_, _) => Clear();
        toolbar.CloseRequested += (_, _) => Dismiss();
        toolbar.Show();
    }

    private void Dismiss()
    {
        toolbar?.Close();
        toolbar = null;

        foreach (var overlay in overlays)
        {
            overlay.Close();
        }

        overlays.Clear();
        history.Clear();
    }

    private void OnAnnotationCompleted(object? sender, Annotation annotation) =>
        history.Push(((OverlayWindow)sender!, annotation));

    private void Undo()
    {
        if (history.TryPop(out var entry))
        {
            entry.Overlay.Remove(entry.Annotation);
        }
    }

    private void Clear()
    {
        foreach (var overlay in overlays)
        {
            overlay.Clear();
        }

        history.Clear();
    }
}
