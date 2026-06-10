using FLaunch2.Models;
using ReactiveUI;

namespace FLaunch2.ViewModels;

public class OptionViewModel : ViewModelBase
{
    private readonly AppSettings _settings;

    public double InitialScoreRate
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public double ScoreIncreaseRate
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public OptionViewModel(AppSettings settings)
    {
        _settings = settings;
        RestoreSettings();
    }

    public void ApplySettings()
    {
        _settings.InitialScoreRate = InitialScoreRate;
        _settings.ScoreIncreaseRate = ScoreIncreaseRate;
    }

    private void RestoreSettings()
    {
        InitialScoreRate = _settings.InitialScoreRate;
        ScoreIncreaseRate = _settings.ScoreIncreaseRate;
    }
}
