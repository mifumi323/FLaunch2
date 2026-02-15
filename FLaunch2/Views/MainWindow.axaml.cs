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
        var item = new Item();
        var vm = new ItemEditViewModel(item, isNew: true);

        vm.OkPressed += (_, _) =>
        {
            // Add the new item to the collection
            if (DataContext is MainViewModel mainVm)
            {
                mainVm.AddItem(vm.ToItem());
            }
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
}
