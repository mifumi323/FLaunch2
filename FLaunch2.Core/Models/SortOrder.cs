namespace FLaunch2.Models;

/// <summary>
/// 並べ替え順序
/// </summary>
public enum SortOrder
{
    /// <summary>スコア順</summary>
    Score,
    /// <summary>利用日時順</summary>
    LastExecuted,
    /// <summary>名前順</summary>
    DisplayName,
    /// <summary>ファイル名順</summary>
    FilePath,
}
