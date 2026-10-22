# ScreenMark

[![CI](https://github.com/KarhuAlex/ScreenMark/actions/workflows/ci.yml/badge.svg)](https://github.com/KarhuAlex/ScreenMark/actions/workflows/ci.yml)

<img width="874" height="448" alt="Preview" src="https://github.com/user-attachments/assets/4ba685d2-3513-4595-a2b2-c13b25c5435f" />

ScreenMark is a small Windows utility for drawing temporary annotations over the desktop and other applications. It is meant for marking up an interface right before you capture it with a screenshot tool such as the Windows Snipping Tool. ScreenMark does not capture, save or edit images itself.

## Features

- Global `Ctrl+0` hotkey toggles a transparent drawing overlay
- Rectangle, ellipse, arrow, freehand and text tools
- Red, green and blue colors
- Undo, redo and clear
- The mouse wheel scrolls the window under the cursor without closing the overlay
- One overlay per monitor, including monitors at negative coordinates and with different DPI scaling
- Annotations stay on screen while a screenshot tool runs and are discarded when the overlay closes
- Runs from the system tray; only one instance can run at a time

## Requirements

- Windows 10 (version 1703) or later
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) to build, .NET 10 Desktop Runtime to run

## Download

Zipped builds are attached to each [release](https://github.com/KarhuAlex/ScreenMark/releases). Extract the zip and run `ScreenMark.exe`. The executable is not code-signed, so Windows SmartScreen may show a warning on first run.

## Build and run

```
dotnet build
dotnet run --project ScreenMark
dotnet test
```

ScreenMark starts in the tray. Press `Ctrl+0` to begin drawing and use **Exit** in the tray menu to quit.

## Usage

1. Press `Ctrl+0`. The overlay appears with the rectangle tool and the red color selected.
2. Drag to draw. Drag in any direction. With the text tool, click where the text should go and type.
3. Start your screenshot tool (for example `Win+Shift+S`) and capture the marked area.
4. Press `Ctrl+0` or click the close button on the toolbar. All annotations are removed.

The toolbar can be moved by dragging its background if it covers the area you want to capture.

## Keyboard shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl+0` | Show or hide the overlay |
| Mouse wheel | Scroll the window under the cursor |
| `Esc` | Cancel the annotation or text being drawn, or close the overlay |
| `Enter` | Place the text being typed |
| `Shift+Enter` | Start a new line in the text |
| `Ctrl+Z` | Undo the last annotation |
| `Ctrl+Y` | Redo the last undone annotation |

## Limitations

- The hotkey is fixed. If another application already uses `Ctrl+0`, ScreenMark reports this and exits.
- While the overlay is shown it receives all mouse input. Close it to interact with the applications underneath.
- Shapes are outlines only. There is no fill, eraser, saving or loading.
- Text uses a fixed font (Segoe UI) and size and cannot be edited after it is placed. Undo it and type it again.
- The toolbar is part of the screen, so it appears in a capture that includes it.
- Monitors with different DPI scaling are handled in code but have not been tested on such a setup.

## License

MIT. See [LICENSE](LICENSE).
