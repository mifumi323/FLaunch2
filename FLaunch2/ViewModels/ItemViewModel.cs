using Avalonia.Media.Imaging;
using FLaunch2.Models;

namespace FLaunch2.ViewModels;

public class ItemViewModel(Item item)
{
    public Item Item { get; } = item;

    public string DisplayName => Item.DisplayName;

    public Bitmap? Icon => null;
}
