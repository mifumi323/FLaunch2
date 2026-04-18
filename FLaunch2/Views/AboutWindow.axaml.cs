using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Reflection;

namespace FLaunch2.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();

        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? "FLaunch2";
        var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright ?? "";

        Title = $"{title}のバージョン情報";
        TitleText.Text = title;
        VersionText.Text = $"Version {version?.ToString(3) ?? "unknown"}";
        CopyrightText.Text = copyright;
    }

    private void OnOkClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
