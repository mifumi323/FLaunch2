using FLaunch2.Models;
using FLaunch2.Services;
using System;
using System.Text.Json;

namespace FLaunch2.Repositories;

/// <summary>
/// アプリケーション設定の永続化を管理するリポジトリ
/// </summary>
public class SettingsRepository
{
    private static readonly JsonSerializerOptions s_options = new()
    {
        WriteIndented = true,
    };

    private static dynamic? _errorService;

    /// <summary>
    /// エラー通知サービスを設定します（オプション）
    /// </summary>
    public static void SetErrorService(dynamic errorService)
    {
        _errorService = errorService;
    }

    /// <summary>
    /// 設定を読み込みます。ファイルが存在しない場合はデフォルト値を返します。
    /// </summary>
    public AppSettings Load()
    {
        try
        {
            var path = DataPathProvider.SettingsPath;
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonSerializer.Deserialize<AppSettings>(json, s_options) ?? new();
            }
        }
        catch (Exception ex)
        {
            var message = "アプリケーション設定の読み込みに失敗しました。デフォルト設定を使用します。";
            try
            {
                _errorService?.NotifyError(message, ex.Message, 0);
            }
            catch { }
        }
        return new();
    }

    /// <summary>
    /// 設定を保存します。
    /// </summary>
    public void Save(AppSettings settings)
    {
        try
        {
            var json = JsonSerializer.Serialize(settings, s_options);
            File.WriteAllText(DataPathProvider.SettingsPath, json);
        }
        catch (Exception ex)
        {
            var message = "アプリケーション設定の保存に失敗しました。";
            try
            {
                _errorService?.NotifyError(message, ex.Message, 2);
            }
            catch { }
        }
    }
}
