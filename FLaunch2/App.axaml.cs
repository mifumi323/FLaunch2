using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using FLaunch2.Models;
using FLaunch2.Repositories;
using FLaunch2.ViewModels;
using FLaunch2.Views;
using System;
using System.IO;
using System.Linq;

namespace FLaunch2;

public partial class App : Application
{
    private IClassicDesktopStyleApplicationLifetime? _desktop;
    private MainWindow? _mainWindow;
    private bool _isExiting;
    private bool _skipSaveSettings;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _desktop = desktop;

            var filePath = desktop.Args?.FirstOrDefault();
            if (!string.IsNullOrEmpty(filePath))
            {
                // コマンドライン引数がある場合は編集ダイアログで追加
                ItemRepository itemRepository = new();
                var allItems = itemRepository.GetAll().ToList();
                SettingsRepository settingsRepository = new();
                var settings = settingsRepository.Load();
                var item = new Item
                {
                    FilePath = filePath,
                    DisplayName = Path.GetFileNameWithoutExtension(filePath),
                    Score = Item.CalculateInitialScore(allItems, settings.InitialScoreRate),
                };
                ItemEditViewModel itemEditViewModel = new(item, Item.GetAllTags(allItems), isNew: true);
                itemEditViewModel.OkPressed += (_, _) =>
                {
                    itemEditViewModel.ApplyTo(item);
                    itemRepository.Upsert(item);
                };
                desktop.MainWindow = new ItemEditWindow
                {
                    DataContext = itemEditViewModel,
                };
            }
            else
            {
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }

            if (TrayIcon.GetIcons(this)?.FirstOrDefault() is TrayIcon trayIcon)
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    trayIcon.Clicked += OnTrayIconClicked;
                }
                else
                {
                    trayIcon.IsVisible = false;
                }
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    internal void ExitApplication(bool skipSaveSettings = false)
    {
        if (_desktop is null)
        {
            return;
        }

        _isExiting = true;
        _skipSaveSettings = skipSaveSettings;

        foreach (var window in _desktop.Windows.Where(x => x != _mainWindow).ToArray())
        {
            window.Close();
        }

        _mainWindow?.Close();
        _desktop.Shutdown();
    }

    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        if (_isExiting)
        {
            return;
        }

        if (_mainWindow is null)
        {
            _mainWindow = new MainWindow
            {
                DataContext = new MainViewModel(),
                Desktop = _desktop,
            };
            _mainWindow.LoadSettings();
            _mainWindow.Closed += OnMainWindowClosed;
        }

        _mainWindow.Display();
    }

    private void OnMainWindowClosed(object? sender, EventArgs e)
    {
        if (sender is MainWindow mainWindow)
        {
            if (!_skipSaveSettings)
            {
                mainWindow.SaveSettings();
            }
            mainWindow.Closed -= OnMainWindowClosed;
        }

        _mainWindow = null;
        _skipSaveSettings = false;
    }
}
