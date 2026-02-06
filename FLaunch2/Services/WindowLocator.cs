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

        var screenRect = screen.WorkingArea;
        var windowFrameSize = window.FrameSize.GetValueOrDefault();

        // ウィンドウサイズをスクリーンサイズに制限
        var newWidth = Math.Min(windowFrameSize.Width, screenRect.Width);
        var newHeight = Math.Min(windowFrameSize.Height, screenRect.Height);
        var scaling = screen.Scaling;
        if (newWidth != windowFrameSize.Width || newHeight != windowFrameSize.Height)
        {
            window.Width = newWidth / scaling;
            window.Height = newHeight / scaling;
            windowFrameSize = new PixelSize((int)newWidth, (int)newHeight).ToSizeWithDpi(scaling);
        }

        var positions = new[]
        {
            new PixelPoint(mousePos.X, mousePos.Y),
            new PixelPoint(mousePos.X - (int)windowFrameSize.Width, mousePos.Y),
            new PixelPoint(mousePos.X, mousePos.Y - (int)windowFrameSize.Height),
            new PixelPoint(mousePos.X - (int)windowFrameSize.Width, mousePos.Y - (int)windowFrameSize.Height)
        };

        foreach (var pos in positions)
        {
            var windowRect = new PixelRect(pos, PixelSize.FromSize(windowFrameSize, scaling));
            if (screenRect.Contains(windowRect))
            {
                window.Position = pos;
                return;
            }
        }

        // どの隅も収まらない場合、スクリーンに収まるように調整
        var finalPos = positions[0];
        if (finalPos.X + windowFrameSize.Width > screenRect.Right)
            finalPos = finalPos.WithX((int)screenRect.Right - (int)windowFrameSize.Width);
        if (finalPos.X < screenRect.X)
            finalPos = finalPos.WithX(screenRect.X);
        if (finalPos.Y + windowFrameSize.Height > screenRect.Bottom)
            finalPos = finalPos.WithY((int)screenRect.Bottom - (int)windowFrameSize.Height);
        if (finalPos.Y < screenRect.Y)
            finalPos = finalPos.WithY(screenRect.Y);

        window.Position = finalPos;
    }
}