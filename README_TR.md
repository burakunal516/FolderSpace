<div align="right">

[English](README.md) | **Türkçe**

</div>

# FolderSpace (MenuFolder)

**C#**, **WPF** ve **.NET 10** kullanılarak geliştirilen, Windows için özelleştirilebilir bir uygulama başlatıcısı.

FolderSpace, uygulamaları ayrı menülerde gruplayarak masaüstünü düzenlemeyi amaçlar. Çok sayıda kısayol arasında arama yapmak yerine yazılım geliştirme araçları, oyunlar veya okul uygulamaları için farklı menüler oluşturabilirsin.

## Özellikler

- **Birden fazla menü:** Uygulamaları farklı gruplar hâlinde yönetme.
- **Özelleştirilebilir menüler:** Menü adlarını, renkleri ve kısayol simgelerini değiştirme.
- **Masaüstü kısayolları:** Her menüyü kendi masaüstü kısayolundan açma.
- **Toplu uygulama seçimi:** Aynı anda birden fazla uygulama ekleme.
- **Uygulama bulma:** Her uygulamanın `.exe` dosyasını tek tek aramadan yüklü uygulamaları seçme.
- **Simge yönetimi:** Masaüstündeki eski kısayol silinse bile uygulama simgesini korumaya yönelik destek.
- **Kaydırma:** Menü ve uygulama seçimi ekranlarında kontrollü kaydırma.
- **Yerel kayıt:** Menü ve uygulama ayarlarını yerel olarak saklama.

## Kullanılan teknolojiler

| Teknoloji | Kullanım amacı |
| --- | --- |
| C# | Uygulama mantığı |
| WPF / XAML | Windows masaüstü arayüzü |
| .NET 10 (`net10.0-windows`) | Uygulama altyapısı |
| JSON | Menü ve ayar verilerini saklama |
| Windows API'leri | Kısayol ve uygulama simgeleri |

## Gereksinimler

- Windows 10 veya 11
- .NET 10 SDK
- .NET masaüstü geliştirme desteğine sahip Visual Studio veya uyumlu bir geliştirme ortamı

## Kurulum ve çalıştırma

1. Projeyi bilgisayarına klonla:

   ```bash
   git clone https://github.com/burakunal516/FolderSpace.git
   cd FolderSpace
   ```

2. `FolderSpace.csproj` dosyasını Visual Studio ile aç veya terminalde aşağıdaki komutları çalıştır:

   ```bash
   dotnet restore
   dotnet run --project FolderSpace.csproj
   ```

3. İlk menünü oluştur, adlandır, uygulamaları seç ve istersen masaüstü kısayolu oluştur.

> **Not:** WPF nedeniyle uygulama yalnızca Windows üzerinde çalışır. Repository'de hazır bir kurulum paketi belgelenmediğinden yukarıdaki adımlar kaynak koddan çalıştırmak içindir.

## Temel proje dosyaları

| Dosya | Görevi |
| --- | --- |
| `App.xaml` / `App.xaml.cs` | Uygulamanın başlangıcı |
| `MainWindow.xaml` / `.cs` | Ana pencere ve menü yönetimi |
| `AppPicker.xaml` / `.cs` | Uygulama seçimi arayüzü |
| `IconPicker.cs` | Simge seçimi desteği |
| `FolderSpace.csproj` | .NET proje yapılandırması |

## Planlanan geliştirmeler

- Sürükle-bırak ile uygulama sıralama
- Ek temalar ve farklı menü düzenleri
- Uygulama bulma ve animasyonların iyileştirilmesi

Bu maddeler **mevcut özellikler değil**, gelecekte yapılabilecek geliştirmelerdir.

## Proje durumu

**Geliştirme aşamasında.** FolderSpace kişisel bir Windows masaüstü uygulaması projesidir.

## Lisans

Repository'de şu anda bir lisans dosyası bulunmuyor.
