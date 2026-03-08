using FLaunch2.Models;
using FLaunch2.Services;
using ReactiveUI;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace FLaunch2.ViewModels;

public class ItemViewModel : ViewModelBase
{
    private readonly IIconExtractor _iconExtractor;
    private AvaloniaBitmap? _icon;
    private bool _iconLoading;

    public ItemViewModel(Item item, IIconExtractor iconExtractor)
    {
        Item = item;
        _iconExtractor = iconExtractor;
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
        Icon = await _iconExtractor.ExtractIconAsync(Item.FilePath);
    }
}
