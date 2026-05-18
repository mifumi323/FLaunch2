using FLaunch2.Models;
using FLaunch2.Services;
using ReactiveUI;

namespace FLaunch2.ViewModels;

public class ImportItemViewModel(Item item, IIconExtractor iconExtractor) : ViewModelBase
{
    public Item Item { get; } = item;
    public ItemViewModel ItemViewModel => new(Item, iconExtractor);

    public string DisplayName => Item.DisplayName;
    public string FilePath => Item.FilePath;

    public bool IsSelected
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;
}
