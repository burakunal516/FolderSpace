using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FolderSpace;

public sealed class IconPicker : Window
{
    public string Symbol { get; private set; }
    public string ColorCode { get; private set; }
    public string ImagePath { get; private set; }
    private readonly TextBlock preview;
    private readonly Border previewBorder;
    private readonly TextBlock fileLabel;

    public IconPicker(string symbol, string color, string imagePath)
    {
        Symbol = symbol; ColorCode = color; ImagePath = imagePath;
        Title = "Menü simgesi";
        Width = 480; Height = 490; MinWidth = 450; MinHeight = 470;
        WindowStartupLocation = WindowStartupLocation.CenterOwner;
        Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(36, 38, 43));
        Foreground = Brushes.White;
        var root = new StackPanel { Margin = new Thickness(20) };
        root.Children.Add(new TextBlock { Text = "Menü simgesini seç", FontSize = 22, FontWeight = FontWeights.SemiBold, Margin = new Thickness(0,0,0,12) });
        preview = new TextBlock { Text = Symbol, FontSize = 44, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
        previewBorder = new Border { Width = 86, Height = 86, CornerRadius = new CornerRadius(19), Background = Brush(ColorCode), Child = preview, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0,0,0,15) };
        root.Children.Add(previewBorder);
        root.Children.Add(new TextBlock { Text = "Hazır simgeler", Foreground = Brushes.LightGray, Margin = new Thickness(0,0,0,6) });
        var symbols = new WrapPanel();
        foreach (var symbolValue in new[] { "🎮", "💻", "📁", "🎵", "📚", "🎨", "🌐", "⭐", "⚙", "🎬", "📷", "🛠" })
        {
            string item = symbolValue;
            var b = new Button { Content = item, FontSize = 24, Width = 57, Height = 48, Margin = new Thickness(3), Background = Brush("#444750"), Foreground = Brushes.White, BorderThickness = new Thickness(0), Cursor = System.Windows.Input.Cursors.Hand };
            b.Click += (_, _) => { Symbol = item; ImagePath = ""; UpdatePreview(); };
            symbols.Children.Add(b);
        }
        root.Children.Add(symbols);
        root.Children.Add(new TextBlock { Text = "Arka plan rengi", Foreground = Brushes.LightGray, Margin = new Thickness(0,14,0,6) });
        var colors = new WrapPanel();
        foreach (var colorValue in new[] { "#5A6170", "#AD454E", "#5751A7", "#357BA4", "#397F6D", "#B0793E", "#A24E8C", "#252A32" })
        {
            string item = colorValue;
            var b = new Button { Width = 42, Height = 33, Margin = new Thickness(4), Background = Brush(item), BorderBrush = Brushes.White, BorderThickness = new Thickness(1) };
            b.Click += (_, _) => { ColorCode = item; UpdatePreview(); };
            colors.Children.Add(b);
        }
        root.Children.Add(colors);
        var choose = new Button { Content = "Bilgisayarımdan PNG / JPG / ICO seç", Height = 37, Margin = new Thickness(0,15,0,4), Background = Brush("#50545E"), Foreground = Brushes.White, BorderThickness = new Thickness(0) };
        choose.Click += (_, _) =>
        {
            var dialog = new OpenFileDialog { Title = "Menü simgesi için resim seç", Filter = "Resimler|*.png;*.jpg;*.jpeg;*.ico" };
            if (dialog.ShowDialog(this) == true) { ImagePath = dialog.FileName; UpdatePreview(); }
        };
        root.Children.Add(choose);
        fileLabel = new TextBlock { Foreground = Brushes.LightGray, TextTrimming = TextTrimming.CharacterEllipsis, FontSize = 11 };
        root.Children.Add(fileLabel);
        var save = new Button { Content = "Simgeyi Kaydet", Height = 42, Margin = new Thickness(0,13,0,0), Background = Brush("#6B707B"), Foreground = Brushes.White, BorderThickness = new Thickness(0), FontWeight = FontWeights.SemiBold };
        save.Click += (_, _) => { DialogResult = true; };
        root.Children.Add(save);
        Content = new ScrollViewer { Content = root, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
        UpdatePreview();
    }
    private static Brush Brush(string hex) => (Brush)new BrushConverter().ConvertFromString(hex)!;
    private void UpdatePreview()
    {
        preview.Text = Symbol;
        previewBorder.Background = Brush(ColorCode);
        fileLabel.Text = string.IsNullOrEmpty(ImagePath) ? "Hazır simge seçili" : "Seçilen resim: " + Path.GetFileName(ImagePath);
    }
}

public static class MenuIconBuilder
{
    public static string EnsureIcon(MenuData menu)
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FolderSpace", "icons");
        Directory.CreateDirectory(dir);
        string ico = Path.Combine(dir, menu.Id + ".ico");
        var bitmap = CreateBitmap(menu);
        using var png = new MemoryStream();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        encoder.Save(png);
        var bytes = png.ToArray();
        using var output = new BinaryWriter(File.Create(ico));
        output.Write((ushort)0); // ICO reserved
        output.Write((ushort)1); // ICO type
        output.Write((ushort)1); // one image
        output.Write((byte)0); // 0 = 256 pixels
        output.Write((byte)0);
        output.Write((byte)0); // color count
        output.Write((byte)0); // reserved
        output.Write((ushort)1); // planes
        output.Write((ushort)32); // bit depth
        output.Write((uint)bytes.Length);
        output.Write((uint)22); // pixel data offset
        output.Write(bytes);
        return ico;
    }

    private static BitmapSource CreateBitmap(MenuData menu)
    {
        const int size = 256;
        if (!string.IsNullOrWhiteSpace(menu.IconFile) && File.Exists(menu.IconFile))
        {
            try
            {
                var source = new BitmapImage();
                source.BeginInit();
                source.UriSource = new Uri(menu.IconFile);
                source.CacheOption = BitmapCacheOption.OnLoad;
                source.EndInit();
                source.Freeze();
                return Render(new Image { Source = source, Stretch = Stretch.UniformToFill, Width = size, Height = size });
            }
            catch { /* fallback to built-in symbol */ }
        }
        var tile = new Border
        {
            Width = size, Height = size, CornerRadius = new CornerRadius(54),
            Background = (Brush)new BrushConverter().ConvertFromString(menu.IconColor)!,
            Child = new TextBlock
            {
                Text = menu.IconSymbol, FontSize = 132, FontFamily = new FontFamily("Segoe UI Emoji"),
                HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center
            }
        };
        return Render(tile);
    }
    private static BitmapSource Render(FrameworkElement element)
    {
        const int size = 256;
        element.Measure(new Size(size, size));
        element.Arrange(new Rect(0, 0, size, size));
        element.UpdateLayout();
        var bitmap = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
        bitmap.Render(element);
        bitmap.Freeze();
        return bitmap;
    }
}
