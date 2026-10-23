using Microsoft.Win32;

namespace ScreenMark;

internal sealed class StartupRegistration
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "ScreenMark";

    private readonly string keyPath;
    private readonly string valueName;
    private readonly string command;

    public StartupRegistration()
        : this(RunKeyPath, ValueName, $"\"{Environment.ProcessPath}\"")
    {
    }

    public StartupRegistration(string keyPath, string valueName, string command)
    {
        this.keyPath = keyPath;
        this.valueName = valueName;
        this.command = command;
    }

    public bool IsEnabled
    {
        get
        {
            using var key = Registry.CurrentUser.OpenSubKey(keyPath);
            return key?.GetValue(valueName) is string value
                && value.Equals(command, StringComparison.OrdinalIgnoreCase);
        }
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(keyPath);
        if (enabled)
        {
            key.SetValue(valueName, command);
        }
        else
        {
            key.DeleteValue(valueName, throwOnMissingValue: false);
        }
    }
}
