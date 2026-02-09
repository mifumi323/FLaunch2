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

        var screenRect = screen.WorkingArea;
        var windowFrameSize = window.FrameSize.GetValueOrDefault();
        var windowFramePixelSize = PixelSize.FromSize(windowFrameSize, scaling);

        // ウィンドウサイズをスクリーンサイズに制限
        var newPixelWidth = Math.Min(windowFramePixelSize.Width, screenRect.Width);
        var newPixelHeight = Math.Min(windowFramePixelSize.Height, screenRect.Height);
        if (newPixelWidth != windowFrameSize.Width || newPixelHeight != windowFrameSize.Height)
        {
            window.Width = newPixelWidth / scaling;
            window.Height = newPixelHeight / scaling;
            windowFramePixelSize = new PixelSize(newPixelWidth, newPixelHeight);
        }

        var positions = new[]
        {
            new PixelPoint(mousePos.X, mousePos.Y),
            new PixelPoint(mousePos.X - windowFramePixelSize.Width, mousePos.Y),
            new PixelPoint(mousePos.X, mousePos.Y - windowFramePixelSize.Height),
            new PixelPoint(mousePos.X - windowFramePixelSize.Width, mousePos.Y - windowFramePixelSize.Height)
        };

        foreach (var pos in positions)
        {
            var windowRect = new PixelRect(pos, windowFramePixelSize);
            if (screenRect.Contains(windowRect))
            {
                window.Position = pos;
                return;
            }
        }

        // どの隅も収まらない場合、スクリーンに収まるように調整
        var finalPos = positions[0];
        if (finalPos.X + windowFramePixelSize.Width > screenRect.Right)
            finalPos = finalPos.WithX((int)screenRect.Right - (int)windowFramePixelSize.Width);
        if (finalPos.X < screenRect.X)
            finalPos = finalPos.WithX(screenRect.X);
        if (finalPos.Y + windowFramePixelSize.Height > screenRect.Bottom)
            finalPos = finalPos.WithY((int)screenRect.Bottom - (int)windowFramePixelSize.Height);
        if (finalPos.Y < screenRect.Y)
            finalPos = finalPos.WithY(screenRect.Y);

        window.Position = finalPos;
    }
}