# ScreenMark

[![CI](https://github.com/KarhuAlex/ScreenMark/actions/workflows/ci.yml/badge.svg)](https://github.com/KarhuAlex/ScreenMark/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

<img width="874" height="448" alt="Preview" src="https://github.com/user-attachments/assets/4ba685d2-3513-4595-a2b2-c13b25c5435f" />

ScreenMark is a small Windows utility for drawing temporary annotations over the desktop and other applications. Use it to mark up an interface before taking a screenshot, to point things out while presenting, or to highlight steps in a screen recording. Annotations disappear when you close the overlay.

## Features

- Global hotkey (`Ctrl+0` by default, configurable) toggles a transparent drawing overlay
- Rectangle, ellipse, arrow, freehand and text tools
- Red, green and blue colors
- Undo, redo and clear
- The mouse wheel scrolls the window under the cursor without closing the overlay
- One overlay per monitor, including monitors at negative coordinates and with different DPI scaling
- Annotations stay on screen while a screenshot tool runs and are discarded when the overlay closes
- The toolbar is excluded from screenshots, screen recordings and screen sharing
- Runs from the system tray; only one instance can run at a time
- Optional start with Windows, switched on or off from the tray menu

## Requirements

- Windows 10 (version 2004) or later
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) to build, .NET 10 Desktop Runtime to run

## Download

Zipped builds are attached to each [release](https://github.com/KarhuAlex/ScreenMark/releases). Extract the zip and run `ScreenMark.exe`. The executable is not code-signed, so Windows SmartScreen may show a warning on first run.

## Build and run

```
dotnet build
dotnet run --project ScreenMark
dotnet test
```

ScreenMark starts in the tray. Press the hotkey (`Ctrl+0` by default) to begin drawing and use **Exit** in the tray menu to quit.

## Usage

1. Press the hotkey. The overlay appears with the rectangle tool and the red color selected.
2. Drag to draw. Drag in any direction. With the text tool, click where the text should go and type.
3. Start your screenshot tool (for example `Win+Shift+S`) and capture the marked area.
4. Press the hotkey again, press `Esc` or click the close button on the toolbar. All annotations are removed.

The toolbar can be moved by dragging its background.

## Changing the hotkey

Right-click the tray icon, choose **Change hotkey…**, press the new shortcut and click **Save**. A shortcut needs Ctrl, Alt or Win plus another key. If another application already uses it, ScreenMark says so and keeps the current one. The choice is stored in `%AppData%\ScreenMark\settings.json`.

If the saved hotkey is taken by another application at startup, ScreenMark asks for a new one instead of starting without it.

## Start with Windows

Tick **Start with Windows** in the tray menu to launch ScreenMark at sign-in. This adds a per-user entry under `HKCU\Software\Microsoft\Windows\CurrentVersion\Run`, and unticking removes it. The entry points at the current location of `ScreenMark.exe`, so tick it again after moving the file.

## Keyboard shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl+0` (default) | Show or hide the overlay |
| Mouse wheel | Scroll the window under the cursor |
| `Esc` | Cancel the annotation or text being drawn, or close the overlay |
| `Enter` | Place the text being typed |
| `Shift+Enter` | Start a new line in the text |
| `Ctrl+Z` | Undo the last annotation |
| `Ctrl+Y` | Redo the last undone annotation |

## Limitations

- While the overlay is shown it receives all mouse input. Close it to interact with the applications underneath.
- Shapes are outlines only. There is no fill, eraser, saving or loading.
- Text uses a fixed font (Segoe UI) and size and cannot be edited after it is placed. Undo it and type it again.
- Monitors with different DPI scaling are handled in code but have not been tested on such a setup.

## License

MIT. See [LICENSE](LICENSE).
