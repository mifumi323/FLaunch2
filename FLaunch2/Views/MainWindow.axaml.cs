using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FLaunch2.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Deactivated += OnDeactivated;
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
