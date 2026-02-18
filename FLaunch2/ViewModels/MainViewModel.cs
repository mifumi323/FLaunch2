using FLaunch2.Models;
using FLaunch2.Repositories;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

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

    internal void ExecuteItem(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.FilePath))
            return;

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = item.FilePath,
                Arguments = item.Arguments,
                WorkingDirectory = string.IsNullOrWhiteSpace(item.WorkingDirectory)
                    ? null
                    : item.WorkingDirectory,
                UseShellExecute = true,
            };
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }
}
