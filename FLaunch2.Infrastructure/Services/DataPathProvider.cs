namespace FLaunch2.Services;

/// <summary>
/// アプリケーションのデータファイルパスを提供するサービス
/// </summary>
public class DataPathProvider
{
    private const string VendorName = "MifuminSoft";
    private const string AppFolderName = "FLaunch2";
    private const string DatabaseFileName = "items.db";
    private const string SettingsFileName = "settings.json";

    /// <summary>
    /// データベースファイルのパスを取得します
    /// </summary>
    public static string DatabasePath => Path.Combine(AppDataFolder, DatabaseFileName);

    /// <summary>
    /// 設定ファイルのパスを取得します
    /// </summary>
    public static string SettingsPath => Path.Combine(AppDataFolder, SettingsFileName);

    /// <summary>
    /// アプリケーションデータフォルダのパスを取得します（フォルダが存在しない場合は作成しません）
    /// </summary>
    public static string AppDataFolderRaw
    {
        get
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appDataPath, VendorName, AppFolderName);
        }
    }

    /// <summary>
    /// アプリケーションデータフォルダのパスを取得します
    /// </summary>
    public static string AppDataFolder
    {
        get
        {
            var appFolder = AppDataFolderRaw;

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            return appFolder;
        }
    }
}
