using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ScreenMark.Drawing;

namespace ScreenMark.UI;

internal sealed class TextEditor
{
    private const double MinimumWidth = 48;
    private const double TextBoxContentOffset = 2;

    private readonly Canvas surface;
    private TextBox? box;
    private Point origin;
    private Color color;

    public TextEditor(Canvas surface)
    {
        this.surface = surface;
    }

    public event EventHandler<Annotation>? Completed;

    public bool IsActive => box is not null;

    public void Begin(Point position, Color textColor)
    {
        origin = position;
        color = textColor;
        box = CreateBox();

        Canvas.SetLeft(box, position.X);
        Canvas.SetTop(box, position.Y);
        surface.Children.Add(box);
        box.Focus();
    }

    public void Commit()
    {
        if (box is not { } editor)
        {
            return;
        }

        var text = editor.Text.TrimEnd();
        Detach(editor);

        if (text.Length > 0)
        {
            Completed?.Invoke(this, Annotation.CreateText(origin, text, color));
        }
    }

    public void Cancel()
    {
        if (box is { } editor)
        {
            Detach(editor);
        }
    }

    private TextBox CreateBox()
    {
        var editor = new TextBox
        {
            AcceptsReturn = true,
            MinWidth = MinimumWidth,
            Background = Brushes.Transparent,
            Foreground = color.ToFrozenBrush(),
            CaretBrush = color.ToFrozenBrush(),
            BorderThickness = new Thickness(0),
            Padding = new Thickness(TextStyle.Inset - TextBoxContentOffset, TextStyle.Inset, 0, 0),
            FontFamily = TextStyle.Family,
            FontSize = TextStyle.Size,
            FontWeight = TextStyle.Weight,
        };

        editor.PreviewKeyDown += OnPreviewKeyDown;
        editor.LostKeyboardFocus += (_, _) => Commit();
        return editor;
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            Cancel();
        }
        else if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
        {
            Commit();
        }
        else
        {
            return;
        }

        e.Handled = true;
    }

    private void Detach(TextBox editor)
    {
        box = null;
        surface.Children.Remove(editor);
    }
}
