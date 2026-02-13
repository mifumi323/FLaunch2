using FLaunch2.Models;
using FLaunch2.Repositories;
using System.Collections.ObjectModel;

namespace FLaunch2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ItemRepository _repository = new();

    public ObservableCollection<Item> Items { get; } = [];

    internal void AddItem(Item item)
    {
        _repository.Upsert(item);
        Items.Add(item);
    }
}
