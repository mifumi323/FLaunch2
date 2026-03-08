using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace FLaunch2.Services;

/// <summary>
/// 関連付けられたアイコンを使用してファイルからアイコンを取得する実装
/// </summary>
public class AssociatedIconExtractor : IIconExtractor
{
    public async Task<AvaloniaBitmap?> ExtractIconAsync(string filePath)
    {
        return await Task.Run(() => ExtractIcon(filePath));
    }

    private static AvaloniaBitmap? ExtractIcon(string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filePath))
                return null;

            using var icon = System.Drawing.Icon.ExtractAssociatedIcon(filePath);
            if (icon is null)
                return null;

            using var bmp = icon.ToBitmap();
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            ms.Position = 0;
            return new AvaloniaBitmap(ms);
        }
        catch
        {
            return null;
        }
    }
}
