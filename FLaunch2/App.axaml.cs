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
    private MainWindow? _mainWindow = null;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
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
                ItemEditViewModel itemEditViewModel = new(item, isNew: true);
                itemEditViewModel.OkPressed += (sender, e) =>
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
                // 通常起動（非表示で起動）
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                _mainWindow = new MainWindow
                {
                    DataContext = new MainViewModel(),
                };
                _mainWindow.LoadSettings();
                _mainWindow.Closed += (_, _) => desktop.Shutdown();
            }

            if (TrayIcon.GetIcons(this)?.FirstOrDefault() is TrayIcon trayIcon)
            {
                if (_mainWindow is not null)
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

    private void OnTrayIconClicked(object? sender, EventArgs e)
    {
        _mainWindow?.Display();
    }
}
