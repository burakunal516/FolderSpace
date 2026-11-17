<div align="right">

**English** | [Türkçe](README_TR.md)

</div>

# FolderSpace (MenuFolder)

A customizable application launcher for Windows, built with **C#**, **WPF**, and **.NET 10**.

FolderSpace helps you keep your desktop organized by grouping applications into separate menus. Instead of searching through many shortcuts, you can create purpose-specific launchers for development tools, games, school applications, and more.

## Features

- **Multiple menus:** Create and manage separate application groups.
- **Customizable menus:** Change menu names, colors, and shortcut icons.
- **Desktop shortcuts:** Open an individual menu from its desktop shortcut.
- **Batch application selection:** Select and add multiple applications in one step.
- **Application discovery:** Find installed applications without manually locating every `.exe` file.
- **Icon handling:** Preserve application icons independently of their original desktop shortcuts.
- **Scrolling:** Navigate menus and the application picker with controlled scrolling.
- **Local configuration:** Store menu and application settings locally.

## Technology stack

| Technology | Usage |
| --- | --- |
| C# | Application logic |
| WPF / XAML | Windows desktop interface |
| .NET 10 (`net10.0-windows`) | Application framework |
| JSON | Local settings and menu data |
| Windows APIs | Shortcuts and application icons |

## Requirements

- Windows 10 or 11
- .NET 10 SDK
- Visual Studio with .NET desktop development support, or another compatible .NET development environment

## Run locally

1. Clone the repository:

   ```bash
   git clone https://github.com/burakunal516/FolderSpace.git
   cd FolderSpace
   ```

2. Open `FolderSpace.csproj` in Visual Studio, or run:

   ```bash
   dotnet restore
   dotnet run --project FolderSpace.csproj
   ```

3. Create your first menu, give it a name, select applications, and optionally create a desktop shortcut.

> **Note:** This is a Windows-only WPF application. A packaged installer is not currently documented in this repository; the steps above run the source project.

## Main source files

| File | Role |
| --- | --- |
| `App.xaml` / `App.xaml.cs` | Application startup |
| `MainWindow.xaml` / `.cs` | Main window and menu management |
| `AppPicker.xaml` / `.cs` | Application selection interface |
| `IconPicker.cs` | Icon selection support |
| `FolderSpace.csproj` | .NET project configuration |

## Planned improvements

- Drag-and-drop application ordering
- More themes and layouts
- Improved application discovery and animations

These are ideas for future versions, **not claims about existing features**.

## Status

**In development.** FolderSpace is a personal Windows desktop application project.

## License

No license file is currently included in the repository.
