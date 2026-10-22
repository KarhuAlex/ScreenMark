using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScreenMark.Drawing;
using ScreenMark.Interop;

namespace ScreenMark.UI;

internal sealed partial class ToolbarWindow : Window
{
    private const int TopMarginDips = 12;

    private readonly DrawingState state;
    private readonly System.Drawing.Rectangle screenBounds;

    public ToolbarWindow(DrawingState state, System.Drawing.Rectangle screenBounds)
    {
        this.state = state;
        this.screenBounds = screenBounds;
        InitializeComponent();

        SelectButton(Tools, state.Tool);
        SelectButton(Colors, state.Color);
    }

    public event EventHandler? UndoRequested;

    public event EventHandler? ClearRequested;

    public event EventHandler? CloseRequested;

    private static void SelectButton(Panel group, object value)
    {
        foreach (var button in group.Children.OfType<RadioButton>())
        {
            button.IsChecked = Equals(button.Tag, value);
        }
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var dpi = VisualTreeHelper.GetDpi(this);
        var widthInPixels = (int)Math.Ceiling(DesiredSize.Width * dpi.DpiScaleX);

        WindowPlacement.MoveToPixel(this, new System.Drawing.Point(
            screenBounds.X + ((screenBounds.Width - widthInPixels) / 2),
            screenBounds.Y + (int)Math.Round(TopMarginDips * dpi.DpiScaleY)));
    }

    private void OnDragHandleMouseDown(object sender, MouseButtonEventArgs e) => DragMove();

    private void OnToolChecked(object sender, RoutedEventArgs e) => state.Tool = (DrawingTool)((RadioButton)e.OriginalSource).Tag;

    private void OnColorChecked(object sender, RoutedEventArgs e) => state.Color = (Color)((RadioButton)e.OriginalSource).Tag;

    private void OnUndoClick(object sender, RoutedEventArgs e) => UndoRequested?.Invoke(this, EventArgs.Empty);

    private void OnClearClick(object sender, RoutedEventArgs e) => ClearRequested?.Invoke(this, EventArgs.Empty);

    private void OnCloseClick(object sender, RoutedEventArgs e) => CloseRequested?.Invoke(this, EventArgs.Empty);
}
