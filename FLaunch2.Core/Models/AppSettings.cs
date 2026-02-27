namespace FLaunch2.Models;

/// <summary>
/// アプリケーション設定
/// </summary>
public class AppSettings
{
    /// <summary>
    /// ウィンドウの幅
    /// </summary>
    public double WindowWidth { get; set; } = 300;

    /// <summary>
    /// ウィンドウの高さ
    /// </summary>
    public double WindowHeight { get; set; } = 300;

    /// <summary>
    /// 初期スコア係数(既存スコアの最大値に対する倍率)
    /// </summary>
    public double InitialScoreRate { get; set; } = 0.1;
}
