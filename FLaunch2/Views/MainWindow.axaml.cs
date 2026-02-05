using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Runtime.InteropServices;

namespace FLaunch2.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Deactivated += OnDeactivated;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorPos(out POINT lpPoint);

    public void Locate()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows以外のプラットフォームでは中央に表示
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return;
        }

        GetCursorPos(out var point);
        var mousePos = new PixelPoint(point.X, point.Y);

        var screen = Screens.ScreenFromPoint(mousePos);
        if (screen == null) return;

        var screenRect = screen.WorkingArea;
        var windowFrameSize = FrameSize.GetValueOrDefault();

        // ウィンドウサイズをスクリーンサイズに制限
        var newWidth = Math.Min(windowFrameSize.Width, screenRect.Width);
        var newHeight = Math.Min(windowFrameSize.Height, screenRect.Height);
        var scaling = screen.Scaling;
        if (newWidth != windowFrameSize.Width || newHeight != windowFrameSize.Height)
        {
            Width = newWidth / scaling;
            Height = newHeight / scaling;
            windowFrameSize = new PixelSize((int)newWidth, (int)newHeight).ToSizeWithDpi(scaling);
        }

        var positions = new[]
        {
            new PixelPoint(mousePos.X, mousePos.Y), // Top-Left
            new PixelPoint(mousePos.X - (int)windowFrameSize.Width, mousePos.Y), // Top-Right
            new PixelPoint(mousePos.X, mousePos.Y - (int)windowFrameSize.Height), // Bottom-Left
            new PixelPoint(mousePos.X - (int)windowFrameSize.Width, mousePos.Y - (int)windowFrameSize.Height) // Bottom-Right
        };

        foreach (var pos in positions)
        {
            var windowRect = new PixelRect(pos, PixelSize.FromSize(windowFrameSize, scaling));
            if (screenRect.Contains(windowRect))
            {
                Position = pos;
                return;
            }
        }

        // どの隅も収まらない場合、スクリーンに収まるように調整
        var finalPos = positions[0]; // Top-Leftを基準に調整
        if (finalPos.X + windowFrameSize.Width > screenRect.Right)
            finalPos = finalPos.WithX((int)screenRect.Right - (int)windowFrameSize.Width);
        if (finalPos.X < screenRect.X)
            finalPos = finalPos.WithX(screenRect.X);
        if (finalPos.Y + windowFrameSize.Height > screenRect.Bottom)
            finalPos = finalPos.WithY((int)screenRect.Bottom - (int)windowFrameSize.Height);
        if (finalPos.Y < screenRect.Y)
            finalPos = finalPos.WithY(screenRect.Y);

        Position = finalPos;
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
