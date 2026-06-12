using FLaunch2.Models;
using FLaunch2.Services;
using ReactiveUI;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace FLaunch2.ViewModels;

public class ItemViewModel(Item item, IIconExtractor iconExtractor) : ViewModelBase
{
    private AvaloniaBitmap? _icon;
    private bool _iconLoading;

    public Item Item { get; } = item;

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
        Icon = await iconExtractor.ExtractIconAsync(Item.FilePath);
    }
}
