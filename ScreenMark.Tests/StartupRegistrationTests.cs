using Microsoft.Win32;

namespace ScreenMark.Tests;

public sealed class StartupRegistrationTests : IDisposable
{
    private const string ValueName = "ScreenMark";
    private const string Command = "\"C:\\Tools\\ScreenMark\\ScreenMark.exe\"";

    private readonly string keyPath = $@"Software\ScreenMark.Tests\{Guid.NewGuid():N}";

    public void Dispose()
    {
        Registry.CurrentUser.DeleteSubKeyTree(@"Software\ScreenMark.Tests", throwOnMissingSubKey: false);
    }

    [Fact]
    public void IsEnabled_IsFalseWhenNothingIsRegistered()
    {
        Assert.False(CreateRegistration(Command).IsEnabled);
    }

    [Fact]
    public void SetEnabled_AddsTheCommandToTheRunKey()
    {
        var registration = CreateRegistration(Command);

        registration.SetEnabled(true);

        Assert.True(registration.IsEnabled);
        using var key = Registry.CurrentUser.OpenSubKey(keyPath);
        Assert.Equal(Command, key?.GetValue(ValueName));
    }

    [Fact]
    public void SetEnabled_RemovesTheEntryAgain()
    {
        var registration = CreateRegistration(Command);
        registration.SetEnabled(true);

        registration.SetEnabled(false);

        Assert.False(registration.IsEnabled);
        using var key = Registry.CurrentUser.OpenSubKey(keyPath);
        Assert.Null(key?.GetValue(ValueName));
    }

    [Fact]
    public void SetEnabled_FalseIsANoOpWhenNothingIsRegistered()
    {
        var registration = CreateRegistration(Command);

        registration.SetEnabled(false);

        Assert.False(registration.IsEnabled);
    }

    [Fact]
    public void IsEnabled_IsFalseForAnEntryPointingAtAnotherLocation()
    {
        CreateRegistration("\"D:\\Old\\ScreenMark.exe\"").SetEnabled(true);
        var current = CreateRegistration(Command);

        Assert.False(current.IsEnabled);

        current.SetEnabled(true);

        Assert.True(current.IsEnabled);
    }

    private StartupRegistration CreateRegistration(string command) => new(keyPath, ValueName, command);
}
