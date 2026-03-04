using Avalonia.Media.Imaging;
using FLaunch2.Models;
using ReactiveUI;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace FLaunch2.ViewModels;

public class ItemViewModel : ViewModelBase
{
    private AvaloniaBitmap? _icon;
    private bool _iconLoading;

    public ItemViewModel(Item item)
    {
        Item = item;
    }

    public Item Item { get; }

    public string DisplayName => Item.DisplayName;

    public AvaloniaBitmap? Icon
    {
        get
        {
            if (_icon is null && !_iconLoading)
            {
                _iconLoading = true;
                _ = LoadIconAsync();
            }
            return _icon;
        }
        private set => this.RaiseAndSetIfChanged(ref _icon, value);
    }

    private async Task LoadIconAsync()
    {
        var bitmap = await Task.Run(() => ExtractIcon(Item.FilePath));
        Icon = bitmap;
    }

    private static AvaloniaBitmap? ExtractIcon(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            using var icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            if (icon is null)
                return null;

            using var bmp = icon.ToBitmap();
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return new AvaloniaBitmap(ms);
        }
        catch
        {
            return null;
        }
    }
}
