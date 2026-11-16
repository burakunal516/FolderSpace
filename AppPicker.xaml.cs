using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using IOPath = System.IO.Path;

namespace FolderSpace;

public class PickEntry : System.ComponentModel.INotifyPropertyChanged
{
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public ImageSource? Icon { get; set; }
    private bool selected;
    public bool Selected { get => selected; set { selected = value; PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(Selected))); } }
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
}

public partial class AppPicker : Window
{
    private readonly List<PickEntry> all = new();
    private readonly ObservableCollection<PickEntry> shown = new();
    public List<PickEntry> SelectedApps => all.Where(a => a.Selected).ToList();

    public AppPicker()
    {
        InitializeComponent();
        AppList.ItemsSource = shown;
        Discover();
        Filter();
    }
    private void Discover()
    {
        var roots = new[]
        {
            Environment.GetFolderPath(Environment.SpecialFolder.StartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu),
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
            Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory)
        };
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var root in roots)
        {
            if (!Directory.Exists(root)) continue;
            try
            {
                foreach (var path in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
                {
                    if (!path.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase) && !path.EndsWith(".url", StringComparison.OrdinalIgnoreCase)) continue;
                    paths.Add(path);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
        }
        foreach (var path in paths.OrderBy(IOPath.GetFileNameWithoutExtension))
        {
            var name = IOPath.GetFileNameWithoutExtension(path);
            if (name.Contains("Uninstall", StringComparison.OrdinalIgnoreCase) || name.Contains("Kaldır", StringComparison.OrdinalIgnoreCase)) continue;
            string resolved = ShortcutResolver.Resolve(path);
            if (!File.Exists(resolved) && !resolved.EndsWith(".url", StringComparison.OrdinalIgnoreCase)) continue;
            all.Add(new PickEntry { Name = name, Path = resolved, Icon = ShellIcon.Get(resolved) ?? ShellIcon.Get(path) });
        }
    }
    private void Filter()
    {
        var query = SearchBox.Text.Trim();
        shown.Clear();
        foreach (var app in all.Where(a => a.Name.Contains(query, StringComparison.CurrentCultureIgnoreCase))) shown.Add(app);
    }
    private void SearchChanged(object sender, TextChangedEventArgs e) { if (shown != null) Filter(); }
    private void SelectAllClick(object sender, RoutedEventArgs e) { foreach (var a in shown) a.Selected = true; }
    private void ClearClick(object sender, RoutedEventArgs e) { foreach (var a in all) a.Selected = false; }
    private void SlowScroll(object sender, MouseWheelEventArgs e)
    {
        if (!AppList.IsMouseOver) return;
        var viewer = FindScrollViewer(AppList);
        if (viewer != null)
        {
            viewer.ScrollToVerticalOffset(viewer.VerticalOffset - Math.Sign(e.Delta) * 6);
            e.Handled = true;
        }
    }
    private static ScrollViewer? FindScrollViewer(DependencyObject parent)
    {
        if (parent is ScrollViewer sv) return sv;
        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var found = FindScrollViewer(System.Windows.Media.VisualTreeHelper.GetChild(parent, i));
            if (found != null) return found;
        }
        return null;
    }
    private void ConfirmClick(object sender, RoutedEventArgs e) { if (SelectedApps.Count == 0) { MessageBox.Show("Önce en az bir uygulama seç."); return; } DialogResult = true; }
}
