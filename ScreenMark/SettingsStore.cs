using System.IO;
using System.Text.Json;

namespace ScreenMark;

internal sealed class SettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    private readonly string path;

    public SettingsStore(string path)
    {
        this.path = path;
    }

    public static string DefaultPath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ScreenMark",
        "settings.json");

    public Hotkey LoadHotkey()
    {
        if (!File.Exists(path))
        {
            return Hotkey.Default;
        }

        try
        {
            var settings = JsonSerializer.Deserialize<StoredSettings>(File.ReadAllText(path), JsonOptions);
            return Hotkey.TryParse(settings?.Hotkey, out var hotkey) ? hotkey : Hotkey.Default;
        }
        catch (JsonException)
        {
            return Hotkey.Default;
        }
    }

    public void SaveHotkey(Hotkey hotkey)
    {
        _ = Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, JsonSerializer.Serialize(new StoredSettings(hotkey.ToString()), JsonOptions));
    }

    private sealed record StoredSettings(string? Hotkey);
}
