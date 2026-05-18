using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FLaunch2.Views;

public partial class ImportWindow : Window
{
    public event EventHandler? ImportClicked;

    public ImportWindow()
    {
        InitializeComponent();
    }

    private void OnImportClicked(object? sender, RoutedEventArgs e)
    {
        ImportClicked?.Invoke(this, EventArgs.Empty);
        Close(true);
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }
}