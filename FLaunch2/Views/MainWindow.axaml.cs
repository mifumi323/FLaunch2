using Avalonia.Controls;
using Avalonia.Interactivity;
using FLaunch2.Services;

namespace FLaunch2.Views;

public partial class MainWindow : Window
{
    private readonly WindowLocator _windowLocator = new();

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
}
