using FLaunch2.Models;
using FLaunch2.Services;
using System;
using System.IO;
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
            Console.Error.WriteLine(ex.Message);
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
            Console.Error.WriteLine(ex.Message);
        }
    }
}
