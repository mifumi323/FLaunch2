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

    internal void ExecuteItem(Item item, bool runas = false)
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
            if (runas)
            {
                psi.Verb = "runas";
            }
            Process.Start(psi);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    internal void DuplicateItem(Item item)
    {
        var clone = new Item
        {
            DisplayName = item.DisplayName,
            FilePath = item.FilePath,
            WorkingDirectory = item.WorkingDirectory,
            Arguments = item.Arguments,
            Comment = item.Comment,
            Tags = [.. item.Tags],
        };
        _repository.Upsert(clone);
        Items.Add(clone);
    }

    internal void DeleteItem(Item item)
    {
        _repository.Delete(item.Id);
        Items.Remove(item);
    }

    internal void UpdateItem(Item item)
    {
        _repository.Upsert(item);

        var index = -1;
        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i].Id == item.Id)
            {
                index = i;
                break;
            }
        }
        if (index >= 0)
        {
            Items[index] = item;
        }
    }
}
