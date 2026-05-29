using Avalonia.Controls;
using Avalonia.Interactivity;

namespace FLaunch2.Views;

public partial class ConfirmWindow : Window
{
    public bool Confirmed { get; private set; }

    public ConfirmWindow(string message)
    {
        InitializeComponent();
        MessageText.Text = message;
    }

    public ConfirmWindow()
    {
        InitializeComponent();
        MessageText.Text = "";
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        Confirmed = true;
        Close();
    }

    private void OnCancelClicked(object? sender, RoutedEventArgs e)
    {
        Confirmed = false;
        Close();
    }
}
