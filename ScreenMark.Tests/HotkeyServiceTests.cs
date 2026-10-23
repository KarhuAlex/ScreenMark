using System.Windows.Input;

namespace ScreenMark.Tests;

public class HotkeyServiceTests
{
    private static readonly Hotkey First = new(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift, Key.F23);
    private static readonly Hotkey Second = new(ModifierKeys.Control | ModifierKeys.Alt | ModifierKeys.Shift, Key.F24);

    [Fact]
    public void TryRegister_ReportsChangesAndAllowsReplacingTheHotkey() => StaThread.Run(() =>
    {
        using var service = new HotkeyService();
        var changes = new List<Hotkey>();
        service.Changed += (_, hotkey) => changes.Add(hotkey);

        Assert.True(service.TryRegister(First));
        Assert.True(service.TryRegister(Second));
        Assert.True(service.TryRegister(Second));

        Assert.Equal([First, Second, Second], changes);
    });

    [Fact]
    public void TryRegister_FailsWhenAnotherOwnerHoldsTheHotkeyAndKeepsTheCurrentOne() => StaThread.Run(() =>
    {
        using var owner = new HotkeyService();
        using var service = new HotkeyService();
        var changes = new List<Hotkey>();
        service.Changed += (_, hotkey) => changes.Add(hotkey);
        Assert.True(owner.TryRegister(First));
        Assert.True(service.TryRegister(Second));

        Assert.False(service.TryRegister(First));

        Assert.Equal([Second], changes);
        Assert.False(owner.TryRegister(Second));
        Assert.True(service.TryRegister(Second));
    });

    [Fact]
    public void Dispose_ReleasesTheHotkey() => StaThread.Run(() =>
    {
        var first = new HotkeyService();
        Assert.True(first.TryRegister(First));
        first.Dispose();

        using var second = new HotkeyService();
        Assert.True(second.TryRegister(First));
    });
}
