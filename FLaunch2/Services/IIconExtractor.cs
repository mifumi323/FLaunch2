using System.Threading.Tasks;
using AvaloniaBitmap = Avalonia.Media.Imaging.Bitmap;

namespace FLaunch2.Services;

/// <summary>
/// ファイルパスからアイコンを取得するサービスのインターフェース
/// </summary>
public interface IIconExtractor
{
    Task<AvaloniaBitmap?> ExtractIconAsync(string filePath);
}
