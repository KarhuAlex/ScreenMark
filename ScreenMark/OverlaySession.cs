using System.Windows.Input;
using ScreenMark.Drawing;
using ScreenMark.UI;
using Screen = System.Windows.Forms.Screen;

namespace ScreenMark;

internal sealed class OverlaySession : IDisposable
{
    private readonly List<OverlayWindow> overlays = [];
    private readonly UndoHistory<(OverlayWindow Overlay, Annotation Annotation)> history = new();
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
            overlay.PreviewKeyDown += OnPreviewKeyDown;
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
        toolbar.RedoRequested += (_, _) => Redo();
        toolbar.ClearRequested += (_, _) => Clear();
        toolbar.CloseRequested += (_, _) => Dismiss();
        toolbar.PreviewKeyDown += OnPreviewKeyDown;
        toolbar.Show();
        toolbar.Activate();
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

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (overlays.Any(overlay => overlay.IsEditingText))
        {
            return;
        }

        switch (e.Key, Keyboard.Modifiers)
        {
            case (Key.Escape, ModifierKeys.None):
                Dismiss();
                break;
            case (Key.Z, ModifierKeys.Control):
                Undo();
                break;
            case (Key.Y, ModifierKeys.Control):
                Redo();
                break;
            default:
                return;
        }

        e.Handled = true;
    }

    private void OnAnnotationCompleted(object? sender, Annotation annotation) =>
        history.Record(((OverlayWindow)sender!, annotation));

    private void Undo()
    {
        if (history.TryUndo(out var entry))
        {
            entry.Overlay.Remove(entry.Annotation);
        }
    }

    private void Redo()
    {
        if (history.TryRedo(out var entry))
        {
            entry.Overlay.Add(entry.Annotation);
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
