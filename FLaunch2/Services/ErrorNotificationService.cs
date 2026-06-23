using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FLaunch2.Services;

/// <summary>
/// エラー通知とログ管理を行うサービス
/// </summary>
public class ErrorNotificationService
{
    public event EventHandler<ErrorNotificationEventArgs>? ErrorOccurred;

    private readonly List<ErrorLogEntry> _errorLog = new();
    private readonly string _logFilePath;
    private const int MaxLogEntries = 100;

    public ErrorNotificationService()
    {
        _logFilePath = Path.Combine(DataPathProvider.AppDataFolder, "error-log.txt");
    }

    /// <summary>
    /// エラーを通知してログに記録します
    /// </summary>
    public void NotifyError(string userMessage, string? technicalDetails = null, ErrorSeverity severity = ErrorSeverity.Warning)
    {
        var entry = new ErrorLogEntry
        {
            Timestamp = DateTimeOffset.Now,
            UserMessage = userMessage,
            TechnicalDetails = technicalDetails,
            Severity = severity,
        };

        _errorLog.Add(entry);

        // ログファイルに追加
        try
        {
            var logContent = FormatLogEntry(entry);
            File.AppendAllText(_logFilePath, logContent + Environment.NewLine);
        }
        catch
        {
            // ログ書き込みに失敗してもアプリは止めない
        }

        // 古いエントリを削除
        if (_errorLog.Count > MaxLogEntries)
        {
            _errorLog.RemoveAt(0);
        }

        // イベント発火
        ErrorOccurred?.Invoke(this, new ErrorNotificationEventArgs(entry));
    }

    /// <summary>
    /// エラーログの詳細を取得します
    /// </summary>
    public string GetFullErrorLog()
    {
        return string.Join(
            Environment.NewLine + Environment.NewLine,
            _errorLog.Select(FormatLogEntry)
        );
    }

    /// <summary>
    /// 直近のエラーを取得します
    /// </summary>
    public IReadOnlyList<ErrorLogEntry> GetRecentErrors(int count = 10)
    {
        return _errorLog.TakeLast(count).ToList().AsReadOnly();
    }

    private static string FormatLogEntry(ErrorLogEntry entry)
    {
        var text = $"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.Severity}] {entry.UserMessage}";
        if (!string.IsNullOrEmpty(entry.TechnicalDetails))
        {
            text += $"\n詳細: {entry.TechnicalDetails}";
        }
        return text;
    }

    /// <summary>
    /// エラーログをクリアします
    /// </summary>
    public void ClearErrorLog()
    {
        _errorLog.Clear();
    }
}

public enum ErrorSeverity
{
    Info,
    Warning,
    Error,
    Critical,
}

public class ErrorLogEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public string UserMessage { get; set; } = "";
    public string? TechnicalDetails { get; set; }
    public ErrorSeverity Severity { get; set; }
}

public class ErrorNotificationEventArgs : EventArgs
{
    public ErrorLogEntry Entry { get; }

    public ErrorNotificationEventArgs(ErrorLogEntry entry)
    {
        Entry = entry;
    }
}
