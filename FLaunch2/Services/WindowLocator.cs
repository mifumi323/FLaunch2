using Avalonia;
using Avalonia.Controls;
using System;
using System.Runtime.InteropServices;

namespace FLaunch2.Services;

public partial class WindowLocator
{
    [StructLayout(LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(out POINT lpPoint);

    public void Locate(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            return;
        }

        GetCursorPos(out var point);
        var mousePos = new PixelPoint(point.X, point.Y);

        var screen = window.Screens.ScreenFromPoint(mousePos);
        if (screen == null) return;
        var scaling = screen.Scaling;

        var screenRect = screen.Bounds;
        var windowPixelWidth = window.Width * scaling;
        var windowPixelHeight = window.Height * scaling;

        // ウィンドウサイズをスクリーンサイズに制限
        var newPixelWidth = Math.Min(windowPixelWidth, screenRect.Width);
        var newPixelHeight = Math.Min(windowPixelHeight, screenRect.Height);
        if (newPixelWidth != windowPixelWidth || newPixelHeight != windowPixelHeight)
        {
            window.Width = newPixelWidth / scaling;
            window.Height = newPixelHeight / scaling;
        }
        window.MaxWidth = screenRect.Width / scaling;
        window.MaxHeight = screenRect.Height / scaling;

        // ウィンドウ位置調整(マウス位置基準に中央に向かって出てくるようにする)
        var centerScreenX = screenRect.X + screenRect.Width / 2;
        var centerScreenY = screenRect.Y + screenRect.Height / 2;
        var newPixelX = mousePos.X <= centerScreenX
            ? Math.Min(mousePos.X, screenRect.Right - newPixelWidth)
            : Math.Max(mousePos.X - newPixelWidth, screenRect.X);
        var newPixelY = mousePos.Y <= centerScreenY
            ? Math.Min(mousePos.Y, screenRect.Bottom - newPixelHeight)
            : Math.Max(mousePos.Y - newPixelHeight, screenRect.Y);
        window.Position = new PixelPoint((int)newPixelX, (int)newPixelY);
    }
}