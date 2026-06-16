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

    public bool ItemEquivalenceDisplayName
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool ItemEquivalenceFilePath
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool ItemEquivalenceWorkingDirectory
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool ItemEquivalenceArguments
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool ExpandEnvironmentVariables
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
        _settings.ItemEquivalence ??= new ItemEquivalenceCondition();
        _settings.ItemEquivalence.DisplayName = ItemEquivalenceDisplayName;
        _settings.ItemEquivalence.FilePath = ItemEquivalenceFilePath;
        _settings.ItemEquivalence.WorkingDirectory = ItemEquivalenceWorkingDirectory;
        _settings.ItemEquivalence.Arguments = ItemEquivalenceArguments;
        _settings.ExpandEnvironmentVariables = ExpandEnvironmentVariables;
    }

    private void RestoreSettings()
    {
        InitialScoreRate = _settings.InitialScoreRate;
        ScoreIncreaseRate = _settings.ScoreIncreaseRate;

        var itemEquivalence = _settings.ItemEquivalence ?? new ItemEquivalenceCondition();
        ItemEquivalenceDisplayName = itemEquivalence.DisplayName;
        ItemEquivalenceFilePath = itemEquivalence.FilePath;
        ItemEquivalenceWorkingDirectory = itemEquivalence.WorkingDirectory;
        ItemEquivalenceArguments = itemEquivalence.Arguments;

        ExpandEnvironmentVariables = _settings.ExpandEnvironmentVariables;
    }
}
