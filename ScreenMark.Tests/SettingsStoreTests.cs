using System.IO;

namespace ScreenMark.Tests;

public sealed class SettingsStoreTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), $"ScreenMark.Tests.{Guid.NewGuid():N}");

    private string SettingsPath => Path.Combine(directory, "nested", "settings.json");

    public void Dispose()
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void LoadHotkey_ReturnsDefaultWhenNothingIsSaved()
    {
        Assert.Equal(Hotkey.Default, new SettingsStore(SettingsPath).LoadHotkey());
    }

    [Fact]
    public void SaveHotkey_CreatesTheFolderAndRoundTrips()
    {
        var store = new SettingsStore(SettingsPath);
        Assert.True(Hotkey.TryParse("Ctrl+Alt+K", out var hotkey));

        store.SaveHotkey(hotkey);

        Assert.Equal(hotkey, new SettingsStore(SettingsPath).LoadHotkey());
    }

    [Theory]
    [InlineData("not json")]
    [InlineData("{}")]
    [InlineData("{\"hotkey\": \"Shift+A\"}")]
    [InlineData("{\"hotkey\": 5}")]
    public void LoadHotkey_FallsBackToDefaultForUnusableFiles(string content)
    {
        _ = Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, content);

        Assert.Equal(Hotkey.Default, new SettingsStore(SettingsPath).LoadHotkey());
    }

    [Fact]
    public void SaveHotkey_ReplacesThePreviousValue()
    {
        var store = new SettingsStore(SettingsPath);
        Assert.True(Hotkey.TryParse("Ctrl+Alt+K", out var first));
        Assert.True(Hotkey.TryParse("Ctrl+Shift+L", out var second));

        store.SaveHotkey(first);
        store.SaveHotkey(second);

        Assert.Equal(second, store.LoadHotkey());
    }
}
