using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FLaunch2.Models;
using FLaunch2.Services;
using FLaunch2.ViewModels;
using System;
using System.Linq;

namespace FLaunch2.Views;

public partial class MainWindow : Window
{
    private readonly WindowLocator _windowLocator = new();
    private ItemEditWindow? _itemEditWindow;
    private AboutWindow? _aboutWindow;
    private ImportWindow? _importWindow;
    private ConfirmWindow? _confirmWindow;
    private OptionWindow? _optionWindow;
    private bool _skipSaveOnClose;

    public MainWindow()
    {
        InitializeComponent();
        Deactivated += OnDeactivated;
    }

    internal void LoadSettings()
    {
        if (DataContext is MainViewModel mainVm)
        {
            mainVm.LoadSettings();
            Width = mainVm.Settings.WindowWidth;
            Height = mainVm.Settings.WindowHeight;
            SortByScoreMenuItem.IsChecked = mainVm.Settings.SortOrder == SortOrder.Score;
            SortByLastExecutedMenuItem.IsChecked = mainVm.Settings.SortOrder == SortOrder.LastExecuted;
            SortByDisplayNameMenuItem.IsChecked = mainVm.Settings.SortOrder == SortOrder.DisplayName;
            SortByFilePathMenuItem.IsChecked = mainVm.Settings.SortOrder == SortOrder.FilePath;
        }
    }

    private void SaveSettings()
    {
        if (DataContext is MainViewModel mainVm)
        {
            mainVm.Settings.WindowWidth = Width;
            mainVm.Settings.WindowHeight = Height;
            mainVm.SaveSettings();
        }
    }

    public void Locate()
    {
        _windowLocator.Locate(this);
    }

    private void OnDeactivated(object? sender, System.EventArgs e)
    {
        Hide();
    }

    private void OnExitClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private async void OnAddClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel mainVm)
        {
            return;
        }

        var item = new Item()
        {
            Score = Item.CalculateInitialScore(mainVm.Items, mainVm.Settings.InitialScoreRate),
        };
        var vm = new ItemEditViewModel(item, Item.GetAllTags(mainVm.Items), isNew: true);

        vm.OkPressed += (_, _) =>
        {
            // Add the new item to the collection
            vm.ApplyTo(item);
            mainVm.AddItem(item);
        };

        OpenItemEditWindow(vm);
    }

    private void OpenItemEditWindow(ItemEditViewModel vm)
    {
        _itemEditWindow?.Close();
        _itemEditWindow = new ItemEditWindow
        {
            DataContext = vm,
        };
        _itemEditWindow.Closed += (_, _) => _itemEditWindow = null;

        _itemEditWindow.Show();
    }

    private void Window_Closed(object? sender, System.EventArgs e)
    {
        _itemEditWindow?.Close();
        _aboutWindow?.Close();
        _importWindow?.Close();
        _confirmWindow?.Close();
        _optionWindow?.Close();
        if (!_skipSaveOnClose)
        {
            SaveSettings();
        }
    }

    internal void Display()
    {
        Locate();
        Show();
        Activate();
    }

    private void Window_Activated(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel mainVm)
        {
            mainVm.Load();
        }
    }

    private void OnItemTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is ItemViewModel itemVm)
        {
            listBox.SelectedItem = null;

            if (DataContext is MainViewModel mainVm)
            {
                mainVm.ExecuteItem(itemVm.Item);
            }
        }
    }

    private void OnContextMenuOpened(object? sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu contextMenu &&
            contextMenu.PlacementTarget is ListBoxItem listBoxItem)
        {
            ItemListBox.SelectedItem = listBoxItem.DataContext;
        }
    }

    private Item? GetSelectedItem()
    {
        return (ItemListBox.SelectedItem as ItemViewModel)?.Item;
    }

    private void OnContextExecuteClicked(object? sender, RoutedEventArgs e)
    {
        if (GetSelectedItem() is { } item && DataContext is MainViewModel mainVm)
        {
            mainVm.ExecuteItem(item);
        }
    }

    private void OnContextRunAsAdminClicked(object? sender, RoutedEventArgs e)
    {
        if (GetSelectedItem() is { } item && DataContext is MainViewModel mainVm)
        {
            mainVm.ExecuteItem(item, runas: true);
        }
    }

    private void OnContextOpenWorkingDirClicked(object? sender, RoutedEventArgs e)
    {
        if (GetSelectedItem() is { } item && DataContext is MainViewModel mainVm)
        {
            mainVm.OpenWorkingDirectory(item);
        }
    }

    private void OnContextOpenFileLocationClicked(object? sender, RoutedEventArgs e)
    {
        if (GetSelectedItem() is { } item && DataContext is MainViewModel mainVm)
        {
            mainVm.OpenFileLocation(item);
        }
    }

    private void OnContextDuplicateClicked(object? sender, RoutedEventArgs e)
    {
        if (GetSelectedItem() is { } item && DataContext is MainViewModel mainVm)
        {
            mainVm.DuplicateItem(item);
        }
    }

    private void OnContextDeleteClicked(object? sender, RoutedEventArgs e)
    {
        if (GetSelectedItem() is { } item && DataContext is MainViewModel mainVm)
        {
            mainVm.DeleteItem(item);
        }
    }

    private void OnContextPropertiesClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel mainVm)
        {
            return;
        }
        if (GetSelectedItem() is { } item)
        {
            var vm = new ItemEditViewModel(item, Item.GetAllTags(mainVm.Items), isNew: false);
            vm.OkPressed += (_, _) =>
            {
                vm.ApplyTo(item);
                mainVm.UpdateItem(item);
            };
            OpenItemEditWindow(vm);
        }
    }

    private void OnContextTagClicked(object? sender, RoutedEventArgs e)
    {
        // TODO: タグ編集機能を実装
    }

    private void OnImportFromStartMenuClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel mainVm)
        {
            return;
        }

        var items = StartMenuReader.ReadItems()
            .OrderBy(i => i.DisplayName)
            .ToArray();

        var importVm = new ImportViewModel(items, mainVm.Items, mainVm.Settings.ItemEquivalence, mainVm.IconExtractor);
        _importWindow?.Close();
        _importWindow = new ImportWindow
        {
            DataContext = importVm,
        };
        _importWindow.ImportClicked += ImportWindow_ImportClicked;
        _importWindow.Closed += (_, _) => _importWindow = null;

        _importWindow.Show();
    }

    private async void OnImportFromFilesClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel mainVm)
        {
            return;
        }

        var storageProvider = StorageProvider;
        var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "インポートするファイルを選択",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("TSVファイル") { Patterns = ["*.tsv"] },
                new FilePickerFileType("すべてのファイル") { Patterns = ["*"] },
            ],
        });

        if (files == null || files.Count == 0)
        {
            return;
        }

        var filePath = files[0].Path.LocalPath;

        var items = FLaunch1Reader.ReadItems(filePath).ToArray();

        var importVm = new ImportViewModel(items, mainVm.Items, mainVm.Settings.ItemEquivalence, mainVm.IconExtractor);
        _importWindow?.Close();
        _importWindow = new ImportWindow
        {
            DataContext = importVm,
        };
        _importWindow.ImportClicked += ImportWindow_ImportClicked;
        _importWindow.Closed += (_, _) => _importWindow = null;

        _importWindow.Show();
    }

    private void ImportWindow_ImportClicked(object? sender, EventArgs e)
    {
        if (sender is ImportWindow importWindow && importWindow.DataContext is ImportViewModel importVm && DataContext is MainViewModel mainVm)
        {
            foreach (var item in importVm.SelectedItems)
            {
                mainVm.AddItem(item);
            }
        }
    }

    private async void OnClearDataAndExitClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel mainVm)
        {
            return;
        }

        Confirm("すべてのデータを消して終了しますか？\nこの操作は元に戻せません。", () =>
        {
            mainVm.DeleteAllData();
            _skipSaveOnClose = true;
            Close();
        });
    }

    private void Confirm(string message, Action onConfirmed)
    {
        _confirmWindow?.Close();
        _confirmWindow = new ConfirmWindow(message);
        _confirmWindow.Closed += (_, _) =>
        {
            if (_confirmWindow.Confirmed)
            {
                onConfirmed();
            }
        };
        _confirmWindow.Show();
    }

    private void OnAboutClicked(object? sender, RoutedEventArgs e)
    {
        _aboutWindow?.Close();
        _aboutWindow = new AboutWindow();
        _aboutWindow.Closed += (_, _) => _aboutWindow = null;
        _aboutWindow.Show();
    }

    private void OnSortByScoreClicked(object? sender, RoutedEventArgs e)
    {
        SetSortOrder(SortOrder.Score);
    }

    private void OnSortByLastExecutedClicked(object? sender, RoutedEventArgs e)
    {
        SetSortOrder(SortOrder.LastExecuted);
    }

    private void OnSortByDisplayNameClicked(object? sender, RoutedEventArgs e)
    {
        SetSortOrder(SortOrder.DisplayName);
    }

    private void OnSortByFilePathClicked(object? sender, RoutedEventArgs e)
    {
        SetSortOrder(SortOrder.FilePath);
    }

    private void SetSortOrder(SortOrder sortOrder)
    {
        if (DataContext is MainViewModel mainVm)
        {
            mainVm.SetSortOrder(sortOrder);
        }
    }

    private void OnOptionClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel mainVm)
        {
            return;
        }
        var vm = new OptionViewModel(mainVm.Settings);
        _optionWindow?.Close();
        _optionWindow = new OptionWindow
        {
            DataContext = vm,
        };
        _optionWindow.Closed += (_, _) => _optionWindow = null;
        _optionWindow.OkClicked += (_, _) => SaveSettings();
        _optionWindow.Show();
    }
}
