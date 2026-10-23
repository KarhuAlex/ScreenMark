using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ScreenMark.UI;

internal sealed partial class HotkeyDialog : Window
{
    private readonly Func<Hotkey, bool> tryApply;
    private Hotkey? candidate;

    public HotkeyDialog(Func<Hotkey, bool> tryApply, string? message)
    {
        this.tryApply = tryApply;
        InitializeComponent();
        ShowMessage(message);
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => Activate();

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        var key = e.Key switch
        {
            Key.System => e.SystemKey,
            Key.ImeProcessed => e.ImeProcessedKey,
            _ => e.Key,
        };

        var pressed = new Hotkey(Keyboard.Modifiers, key);
        if (!pressed.IsValid)
        {
            return;
        }

        candidate = pressed;
        Shortcut.Text = pressed.ToString();
        Shortcut.Foreground = Brushes.White;
        SaveButton.IsEnabled = true;
        ShowMessage(null);
        e.Handled = true;
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (candidate is not { } hotkey)
        {
            return;
        }

        if (tryApply(hotkey))
        {
            DialogResult = true;
        }
        else
        {
            ShowMessage($"{hotkey} is already in use by another application.");
        }
    }

    private void ShowMessage(string? text)
    {
        Message.Text = text;
        Message.Visibility = text is null ? Visibility.Collapsed : Visibility.Visible;
    }
}
