using FLaunch2.Models;
using FLaunch2.Repositories;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace FLaunch2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ItemRepository _repository = new();

    public ObservableCollection<Item> Items
    {
        get; private set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    internal void AddItem(Item item)
    {
        _repository.Upsert(item);
        Items.Add(item);
    }

    internal void Load()
    {
        Items = new ObservableCollection<Item>(_repository.GetAll());
    }
}
