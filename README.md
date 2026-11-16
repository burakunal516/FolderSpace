# MenuFolder

**A customizable desktop application launcher for Windows, built with C# and WPF.**

MenuFolder helps you organize your desktop by grouping applications into separate, customizable menus. Instead of keeping dozens of shortcuts on your desktop, you can create dedicated launchers for games, development tools, school applications, and more.

## Features

- **Multiple Independent Menus:** Create separate launchers for different application categories.
- **Custom Desktop Shortcuts:** Open each menu directly from its own desktop shortcut.
- **Custom Icons:** Choose your own PNG, JPG, or ICO images for menu shortcuts.
- **Theme Customization:** Personalize menus with different colors and icons.
- **Batch Application Selection:** Add multiple installed applications at once.
- **Application Search:** Find applications without manually locating executable files.
- **Persistent Application Icons:** Keep application icons even after removing their desktop shortcuts.
- **Adjustable Interface:** Rename menus and organize applications independently.
- **Smooth Scrolling:** Controlled scrolling in both the main menu and application selection window.
- **Local Data Storage:** Save your menus and applications locally.

## Technologies

| Technology | Purpose |
|---|---|
| C# | Application logic |
| WPF | Desktop user interface |
| XAML | Interface design |
| .NET 10 | Application framework |
| JSON | Local data persistence |
| Windows APIs | Shortcut and icon management |

## Getting Started

### Requirements

- Windows 10 or Windows 11
- Visual Studio 2022 or later with WPF development support
- .NET 10 SDK

### Installation

1. Clone the repository:

```bash
git clone https://github.com/YOUR_USERNAME/MenuFolder.git
```

2. Open `FolderSpace.csproj` in Visual Studio.
3. Restore dependencies and build the project.
4. Press `F5` to run MenuFolder.

### Usage

1. Create a new menu, such as **Games** or **Development**.
2. Customize its name, color, and icon.
3. Select applications from the application list.
4. Add multiple applications at once.
5. Create a desktop shortcut for the menu.
6. Launch your applications from your organized desktop.

## Project Structure

The project uses WPF and XAML for its interface, C# for application management, and JSON files for saving menu configurations.

## Future Improvements

- Drag-and-drop application reordering
- Additional themes and customization options
- Improved application discovery
- Enhanced animations and transitions
- More flexible menu layouts

## Project Status

**In development**

MenuFolder is a personal desktop application project. New features and improvements are being developed.

## License

No license has been specified yet.

---

Built with C# and WPF.
