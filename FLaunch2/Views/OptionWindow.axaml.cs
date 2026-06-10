using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using FLaunch2.ViewModels;

namespace FLaunch2.Views;

public partial class OptionWindow : Window
{
    public event EventHandler? OkClicked;

    public OptionWindow()
    {
        InitializeComponent();
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is OptionViewModel vm)
        {
            vm.ApplySettings();
        }

        OkClicked?.Invoke(this, EventArgs.Empty);
        Close();
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
