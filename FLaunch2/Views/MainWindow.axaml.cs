using Avalonia.Controls;
using Avalonia.Interactivity;
using FLaunch2.Models;
using FLaunch2.Services;
using FLaunch2.ViewModels;
using System;

namespace FLaunch2.Views;

public partial class MainWindow : Window
{
    private readonly WindowLocator _windowLocator = new();
    private ItemEditWindow? _itemEditWindow;

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
        var vm = new ItemEditViewModel(item, isNew: true);

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
        SaveSettings();
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
        if (sender is ListBox listBox && listBox.SelectedItem is Item item)
        {
            listBox.SelectedItem = null;

            if (DataContext is MainViewModel mainVm)
            {
                mainVm.ExecuteItem(item);
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
        return ItemListBox.SelectedItem as Item;
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
        if (GetSelectedItem() is { } item)
        {
            var vm = new ItemEditViewModel(item, isNew: false);
            vm.OkPressed += (_, _) =>
            {
                if (DataContext is MainViewModel mainVm)
                {
                    vm.ApplyTo(item);
                    mainVm.UpdateItem(item);
                }
            };
            OpenItemEditWindow(vm);
        }
    }

    private void OnContextTagClicked(object? sender, RoutedEventArgs e)
    {
        // TODO: タグ編集機能を実装
    }
}
