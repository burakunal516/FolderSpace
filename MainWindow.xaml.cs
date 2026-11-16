using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IOPath = System.IO.Path;

namespace FolderSpace;

public class AppEntry
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public ImageSource? Icon { get; set; }
}
public class MenuData
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Yeni Menü";
    public string IconFile { get; set; } = "";
    public string IconSymbol { get; set; } = "📁";
    public string IconColor { get; set; } = "#626A79";
    public List<AppRecord> Apps { get; set; } = new();
}
public class AppRecord
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public string IconCache { get; set; } = "";
}
public class DataStore
{
    public List<MenuData> Menus { get; set; } = new();
}

public partial class MainWindow : Window
{
    private static readonly string SaveFile = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FolderSpace", "menus.json");
    private DataStore store = new();
    private MenuData current = null!;
    private readonly ObservableCollection<AppEntry> visibleApps = new();

    public MainWindow()
    {
        InitializeComponent();
        AppsList.ItemsSource = visibleApps;
        LoadData();
        var args = Environment.GetCommandLineArgs();
        string? id = Array.IndexOf(args, "--menu") is int index && index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
        current = store.Menus.FirstOrDefault(m => m.Id == id) ?? store.Menus[0];
        Refresh();
    }

    private void LoadData()
    {
        try
        {
            if (File.Exists(SaveFile)) store = JsonSerializer.Deserialize<DataStore>(File.ReadAllText(SaveFile)) ?? new();
        }
        catch { MessageBox.Show("Kayıt dosyası okunamadı. Yeni bir liste oluşturulacak."); }
        bool migrated = false;
        foreach (var menu in store.Menus)
        foreach (var app in menu.Apps)
        {
            if (File.Exists(app.Path) && app.Path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
            {
                string resolved = ShortcutResolver.Resolve(app.Path);
                if (!string.Equals(resolved, app.Path, StringComparison.OrdinalIgnoreCase)) { app.Path = resolved; migrated = true; }
            }
            // Upgrade old icon caches, which could contain the generic PNG-file icon.
            if (File.Exists(app.Path) && (string.IsNullOrEmpty(app.IconCache) || !app.IconCache.EndsWith(".v2.png", StringComparison.OrdinalIgnoreCase) || !File.Exists(app.IconCache)))
            {
                var updated = AppIconCache.Save(app.Path);
                if (!string.IsNullOrEmpty(updated)) { app.IconCache = updated; migrated = true; }
            }
        }
        if (migrated) Save();
        if (store.Menus.Count == 0) { store.Menus.Add(new MenuData { Name = "Uygulamalarım" }); Save(); }
    }
    private void Save()
    {
        Directory.CreateDirectory(IOPath.GetDirectoryName(SaveFile)!);
        File.WriteAllText(SaveFile, JsonSerializer.Serialize(store, new JsonSerializerOptions { WriteIndented = true }));
    }
    private void Refresh()
    {
        TitleBox.Text = current.Name;
        Title = current.Name;
        visibleApps.Clear();
        foreach (var a in current.Apps)
            visibleApps.Add(new AppEntry { Name = a.Name, Path = a.Path, Icon = AppIconCache.Load(a.IconCache) ?? ShellIcon.Get(a.Path) });
    }
    private void SaveTitle(object sender, RoutedEventArgs e)
    {
        if (current == null) return;
        var title = TitleBox.Text.Trim();
        if (title.Length == 0) { TitleBox.Text = current.Name; return; }
        current.Name = title;
        Title = title;
        Save();
        UpdateShortcutIfPresent();
    }
    private void TitleKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { SaveTitle(sender, e); Keyboard.ClearFocus(); }
    }
    private void DragHeader(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is TextBox) return;
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }
    private void CloseClick(object sender, RoutedEventArgs e) => Close();
    private void NewMenu(object sender, RoutedEventArgs e)
    {
        var name = Prompt("Yeni menünün adı", "Yeni Menü");
        if (string.IsNullOrWhiteSpace(name)) return;
        current = new MenuData { Name = name.Trim() };
        store.Menus.Add(current);
        Save(); Refresh();
        if (MessageBox.Show("Bu menü için masaüstü kısayolu oluşturulsun mu?", "FolderSpace", MessageBoxButton.YesNo) == MessageBoxResult.Yes) CreateShortcut();
    }
    private void ShowMenus(object sender, RoutedEventArgs e)
    {
        var window = new Window { Title = "Menülerim", Width = 330, Height = 390, WindowStartupLocation = WindowStartupLocation.CenterOwner, Owner = this, Background = new SolidColorBrush(Color.FromRgb(34, 36, 41)) };
        var panel = new StackPanel { Margin = new Thickness(15) };
        panel.Children.Add(new TextBlock { Text = "Açmak istediğin menüyü seç", Foreground = Brushes.White, Margin = new Thickness(4, 0, 0, 12) });
        foreach (var menu in store.Menus.ToList())
        {
            var b = new Button { Content = menu.Name, Margin = new Thickness(0, 3, 0, 3), Padding = new Thickness(12), Background = new SolidColorBrush(Color.FromRgb(65, 68, 76)), Foreground = Brushes.White, Tag = menu };
            b.Click += (_, _) => { current = (MenuData)b.Tag; Refresh(); window.Close(); };
            panel.Children.Add(b);
        }
        window.Content = new ScrollViewer { Content = panel };
        window.ShowDialog();
    }
    private void AddApps(object sender, RoutedEventArgs e)
    {
        var picker = new AppPicker { Owner = this };
        if (picker.ShowDialog() != true) return;
        foreach (var a in picker.SelectedApps)
        {
            if (current.Apps.Any(x => string.Equals(x.Path, a.Path, StringComparison.OrdinalIgnoreCase))) continue;
            current.Apps.Add(new AppRecord { Name = a.Name, Path = a.Path, IconCache = AppIconCache.Save(a.Path) });
        }
        Save(); Refresh();
    }
    private void LaunchApp(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: AppEntry app }) return;
        try
        {
            Process.Start(new ProcessStartInfo { FileName = app.Path, UseShellExecute = true, WorkingDirectory = IOPath.GetDirectoryName(app.Path) ?? "" });
            Close();
        }
        catch (Exception ex) { MessageBox.Show("Uygulama açılamadı: " + ex.Message); }
    }
    private void RemoveApp(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: AppEntry app }) return;
        current.Apps.RemoveAll(a => a.Path == app.Path);
        Save(); Refresh();
    }
    private void DesktopShortcut(object sender, RoutedEventArgs e) => CreateShortcut();
    private void ChooseIcon(object sender, RoutedEventArgs e)
    {
        var picker = new IconPicker(current.IconSymbol, current.IconColor, current.IconFile) { Owner = this };
        if (picker.ShowDialog() != true) return;
        current.IconSymbol = picker.Symbol;
        current.IconColor = picker.ColorCode;
        current.IconFile = picker.ImagePath;
        Save();
        UpdateShortcutIfPresent();
        MessageBox.Show("Simge kaydedildi. Masaüstü kısayolunun simgesi güncellendi (varsa).", "FolderSpace");
    }
    private string ShortcutPath()
    {
        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        string fileName = string.Concat(current.Name.Select(c => IOPath.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        return IOPath.Combine(desktop, fileName + ".lnk");
    }
    private void UpdateShortcutIfPresent()
    {
        if (File.Exists(ShortcutPath())) CreateShortcut(false);
    }
    private void SlowScroll(object sender, MouseWheelEventArgs e)
    {
        if (sender is ScrollViewer scroll)
        {
            scroll.ScrollToVerticalOffset(scroll.VerticalOffset - Math.Sign(e.Delta) * 6);
            e.Handled = true;
        }
    }
    private void CreateShortcut(bool notify = true)
    {
        try
        {
            string shortcutPath = ShortcutPath();
            Type? type = Type.GetTypeFromProgID("WScript.Shell");
            if (type == null) throw new InvalidOperationException("Windows kısayol servisi bulunamadı.");
            dynamic shell = Activator.CreateInstance(type)!;
            dynamic shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = Environment.ProcessPath ?? Process.GetCurrentProcess().MainModule!.FileName;
            shortcut.Arguments = "--menu " + current.Id;
            shortcut.WorkingDirectory = AppContext.BaseDirectory;
            shortcut.Description = "FolderSpace: " + current.Name;
            string iconPath = MenuIconBuilder.EnsureIcon(current);
            shortcut.IconLocation = iconPath + ",0";
            shortcut.Save();
            if (notify) MessageBox.Show("Masaüstüne '" + current.Name + "' kısayolu eklendi.");
        }
        catch (Exception ex) { MessageBox.Show("Kısayol oluşturulamadı: " + ex.Message); }
    }
    private string? Prompt(string title, string defaultValue)
    {
        var w = new Window { Title = title, Width = 340, Height = 155, Owner = this, WindowStartupLocation = WindowStartupLocation.CenterOwner, ResizeMode = ResizeMode.NoResize };
        var box = new TextBox { Text = defaultValue, Margin = new Thickness(12), FontSize = 16 };
        var button = new Button { Content = "Oluştur", Margin = new Thickness(12, 0, 12, 12), Height = 32, IsDefault = true };
        button.Click += (_, _) => w.DialogResult = true;
        w.Content = new StackPanel { Children = { box, button } };
        return w.ShowDialog() == true ? box.Text : null;
    }
}

internal static class ShellIcon
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)] public string szTypeName;
    }
    [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr SHGetFileInfo(string path, uint attributes, out SHFILEINFO info, uint cbFileInfo, uint flags);
    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr icon);
    public static ImageSource? Get(string path)
    {
        try
        {
            const uint SHGFI_ICON = 0x100, SHGFI_LARGEICON = 0;
            if (SHGetFileInfo(path, 0, out var info, (uint)Marshal.SizeOf<SHFILEINFO>(), SHGFI_ICON | SHGFI_LARGEICON) == IntPtr.Zero || info.hIcon == IntPtr.Zero) return null;
            try
            {
                var bitmap = Imaging.CreateBitmapSourceFromHIcon(info.hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                bitmap.Freeze(); return bitmap;
            }
            finally { DestroyIcon(info.hIcon); }
        }
        catch { return null; }
    }
}

internal static class ShortcutResolver
{
    public static string Resolve(string path)
    {
        if (!path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase)) return path;
        try
        {
            var type = Type.GetTypeFromProgID("WScript.Shell");
            if (type == null) return path;
            dynamic shell = Activator.CreateInstance(type)!;
            dynamic link = shell.CreateShortcut(path);
            string target = (string)link.TargetPath;
            return !string.IsNullOrWhiteSpace(target) && File.Exists(target) ? target : path;
        }
        catch { return path; }
    }
}

internal static class AppIconCache
{
    public static ImageSource? Load(string cachePath)
    {
        if (string.IsNullOrWhiteSpace(cachePath) || !File.Exists(cachePath)) return null;
        try
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(cachePath, UriKind.Absolute);
            image.EndInit();
            image.Freeze();
            return image;
        }
        catch { return null; }
    }

    public static string Save(string path)
    {
        try
        {
            var folder = IOPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FolderSpace", "app-icons");
            Directory.CreateDirectory(folder);
            var key = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(path.ToLowerInvariant())));
            var file = IOPath.Combine(folder, key + ".v2.png");
            if (File.Exists(file)) return file;
            var source = ShellIcon.Get(path);
            if (source is not BitmapSource bitmap) return "";
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using var stream = File.Create(file);
            encoder.Save(stream);
            return file;
        }
        catch { return ""; }
    }
}
