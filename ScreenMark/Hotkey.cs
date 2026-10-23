using System.Globalization;
using System.Windows.Input;

namespace ScreenMark;

internal readonly record struct Hotkey(ModifierKeys Modifiers, Key Key)
{
    private const ModifierKeys TriggerModifiers = ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Windows;

    private static readonly (string Name, ModifierKeys Modifier)[] ModifierNames =
    [
        ("Ctrl", ModifierKeys.Control),
        ("Alt", ModifierKeys.Alt),
        ("Shift", ModifierKeys.Shift),
        ("Win", ModifierKeys.Windows),
    ];

    private static readonly HashSet<Key> NonTriggerKeys =
    [
        Key.None,
        Key.System,
        Key.ImeProcessed,
        Key.DeadCharProcessed,
        Key.LeftCtrl,
        Key.RightCtrl,
        Key.LeftAlt,
        Key.RightAlt,
        Key.LeftShift,
        Key.RightShift,
        Key.LWin,
        Key.RWin,
    ];

    public static Hotkey Default => new(ModifierKeys.Control, Key.D0);

    public bool IsValid => (Modifiers & TriggerModifiers) != 0 && !NonTriggerKeys.Contains(Key);

    public static bool TryParse(string? text, out Hotkey hotkey)
    {
        hotkey = default;
        var parts = text?.Split('+', StringSplitOptions.TrimEntries) ?? [];
        if (parts.Length < 2 || !TryParseKey(parts[^1], out var key))
        {
            return false;
        }

        var modifiers = ModifierKeys.None;
        foreach (var name in parts[..^1])
        {
            var match = ModifierNames.FirstOrDefault(entry => entry.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (match.Name is null)
            {
                return false;
            }

            modifiers |= match.Modifier;
        }

        hotkey = new Hotkey(modifiers, key);
        return hotkey.IsValid;
    }

    public override string ToString()
    {
        var modifiers = Modifiers;
        var names = ModifierNames.Where(entry => modifiers.HasFlag(entry.Modifier)).Select(entry => entry.Name);
        return string.Join('+', names.Append(FormatKey(Key)));
    }

    private static string FormatKey(Key key) =>
        key is >= Key.D0 and <= Key.D9 ? (key - Key.D0).ToString(CultureInfo.InvariantCulture) : key.ToString();

    private static bool TryParseKey(string text, out Key key)
    {
        key = Key.None;
        if (text.Length == 0)
        {
            return false;
        }

        if (text.Length == 1 && char.IsAsciiDigit(text[0]))
        {
            key = Key.D0 + (text[0] - '0');
            return true;
        }

        return !char.IsAsciiDigit(text[0]) && Enum.TryParse(text, ignoreCase: true, out key) && Enum.IsDefined(key);
    }
}
