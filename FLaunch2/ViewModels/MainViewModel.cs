using FLaunch2.Models;
using FLaunch2.Repositories;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FLaunch2.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ItemRepository _repository = new();
    private readonly SettingsRepository _settingsRepository = new();

    public AppSettings Settings { get; private set; } = new();

    public ObservableCollection<Item> Items
    {
        get; private set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    public IEnumerable<Item> DisplayItems
    {
        get; private set => this.RaiseAndSetIfChanged(ref field, value);
    } = [];

    public MainViewModel()
    {
        PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(Items) || e.PropertyName == nameof(Settings))
            {
                UpdateDiaplayItems();
            }
        };
    }

    private void UpdateDiaplayItems()
    {
        DisplayItems = [
            .. Items
            .OrderByDescending(x => x.Score)
            .ThenByDescending(x => x.LastExecuted)
        ];
    }

    internal void AddItem(Item item)
    {
        _repository.Upsert(item);
        Items.Add(item);
    }

    internal void Load()
    {
        Items = new ObservableCollection<Item>(_repository.GetAll());
    }

    internal void LoadSettings()
    {
        Settings = _settingsRepository.Load();
    }

    internal void SaveSettings()
    {
        _settingsRepository.Save(Settings);
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

            item.LastExecuted = DateTimeOffset.Now;
            if (item.IncreaseScore(Items, Settings.ScoreIncreaseRate))
            {
                UpdateItems(Items);
            }
            else
            {
                UpdateItem(item);
            }
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
    }

    private void UpdateItems(ObservableCollection<Item> items)
    {
        _repository.UpsertMany(items);
    }

    internal void OpenWorkingDirectory(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.WorkingDirectory))
            return;

        Process.Start(new ProcessStartInfo
        {
            FileName = item.WorkingDirectory,
            UseShellExecute = true,
        });
    }

    internal void OpenFileLocation(Item item)
    {
        if (string.IsNullOrWhiteSpace(item.FilePath))
            return;

        var dir = Path.GetDirectoryName(item.FilePath);
        if (!string.IsNullOrWhiteSpace(dir))
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = dir,
                UseShellExecute = true,
            });
        }
    }
}
