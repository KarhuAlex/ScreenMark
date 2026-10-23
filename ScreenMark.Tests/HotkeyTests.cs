using System.Windows.Input;

namespace ScreenMark.Tests;

public class HotkeyTests
{
    [Fact]
    public void Default_IsCtrlZero()
    {
        Assert.Equal(new Hotkey(ModifierKeys.Control, Key.D0), Hotkey.Default);
        Assert.Equal("Ctrl+0", Hotkey.Default.ToString());
    }

    [Theory]
    [InlineData(ModifierKeys.Control, Key.D0, "Ctrl+0")]
    [InlineData(ModifierKeys.Control | ModifierKeys.Shift, Key.K, "Ctrl+Shift+K")]
    [InlineData(ModifierKeys.Alt | ModifierKeys.Control, Key.F9, "Ctrl+Alt+F9")]
    [InlineData(ModifierKeys.Windows | ModifierKeys.Alt | ModifierKeys.Shift | ModifierKeys.Control, Key.OemComma, "Ctrl+Alt+Shift+Win+OemComma")]
    public void ToString_ListsModifiersInFixedOrder(ModifierKeys modifiers, Key key, string expected)
    {
        Assert.Equal(expected, new Hotkey(modifiers, key).ToString());
    }

    [Theory]
    [InlineData(ModifierKeys.Control, Key.D0)]
    [InlineData(ModifierKeys.Control | ModifierKeys.Shift, Key.K)]
    [InlineData(ModifierKeys.Alt, Key.F12)]
    [InlineData(ModifierKeys.Windows | ModifierKeys.Control, Key.OemComma)]
    [InlineData(ModifierKeys.Control, Key.NumPad5)]
    public void TryParse_RoundTripsFormattedHotkeys(ModifierKeys modifiers, Key key)
    {
        var original = new Hotkey(modifiers, key);

        Assert.True(Hotkey.TryParse(original.ToString(), out var parsed));
        Assert.Equal(original, parsed);
    }

    [Theory]
    [InlineData("ctrl+alt+x")]
    [InlineData(" Ctrl + Alt + X ")]
    public void TryParse_IgnoresCaseAndSpacing(string text)
    {
        Assert.True(Hotkey.TryParse(text, out var parsed));
        Assert.Equal(new Hotkey(ModifierKeys.Control | ModifierKeys.Alt, Key.X), parsed);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("0")]
    [InlineData("Ctrl")]
    [InlineData("Ctrl+")]
    [InlineData("Shift+A")]
    [InlineData("Ctrl+Banana")]
    [InlineData("Hyper+A")]
    [InlineData("Ctrl+Shift")]
    [InlineData("Ctrl+99")]
    public void TryParse_RejectsInvalidText(string? text)
    {
        Assert.False(Hotkey.TryParse(text, out _));
    }

    [Theory]
    [InlineData(ModifierKeys.None, Key.D0, false)]
    [InlineData(ModifierKeys.Shift, Key.A, false)]
    [InlineData(ModifierKeys.Control, Key.LeftCtrl, false)]
    [InlineData(ModifierKeys.Control, Key.System, false)]
    [InlineData(ModifierKeys.Control, Key.None, false)]
    [InlineData(ModifierKeys.Control, Key.A, true)]
    [InlineData(ModifierKeys.Alt, Key.F1, true)]
    [InlineData(ModifierKeys.Windows, Key.D1, true)]
    public void IsValid_RequiresATriggerModifierAndARealKey(ModifierKeys modifiers, Key key, bool expected)
    {
        Assert.Equal(expected, new Hotkey(modifiers, key).IsValid);
    }
}
