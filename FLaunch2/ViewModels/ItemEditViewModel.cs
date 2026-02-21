using Avalonia.Controls;
using FLaunch2.Models;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace FLaunch2.ViewModels;

public class ItemEditViewModel : ViewModelBase
{
    private string _displayName = string.Empty;
    private string _filePath = string.Empty;
    private string _arguments = string.Empty;
    private string _workingDirectory = string.Empty;
    private string _comment = string.Empty;
    private string _newTag = string.Empty;

    public ItemEditViewModel()
    {
        OkCommand = ReactiveCommand.Create<Window>(OnOk);
        CancelCommand = ReactiveCommand.Create<Window>(OnCancel);
        ResolveLinkCommand = ReactiveCommand.Create(OnResolveLink);
    }

    public ItemEditViewModel(Item item, bool isNew) : this()
    {
        IsNew = isNew;

        DisplayName = item.DisplayName;
        FilePath = item.FilePath;
        Arguments = item.Arguments;
        WorkingDirectory = item.WorkingDirectory;
        Comment = item.Comment;

        // TODO: 既存タグ一覧をリポジトリから読み込んで TagItems を構築する
        foreach (var tag in item.Tags)
        {
            TagItems.Add(new TagItemViewModel(tag, isChecked: true));
        }
    }

    public bool IsNew { get; }

    /// <summary>編集結果が確定されたかどうか</summary>
    public bool IsConfirmed { get; private set; }

    /// <summary>OK が押されたときに発火するイベント</summary>
    public event EventHandler? OkPressed;

    /// <summary>キャンセルが押されたときに発火するイベント</summary>
    public event EventHandler? CancelPressed;

    public string DisplayName
    {
        get => _displayName;
        set => this.RaiseAndSetIfChanged(ref _displayName, value);
    }

    public string FilePath
    {
        get => _filePath;
        set => this.RaiseAndSetIfChanged(ref _filePath, value);
    }

    public string Arguments
    {
        get => _arguments;
        set => this.RaiseAndSetIfChanged(ref _arguments, value);
    }

    public string WorkingDirectory
    {
        get => _workingDirectory;
        set => this.RaiseAndSetIfChanged(ref _workingDirectory, value);
    }

    public string Comment
    {
        get => _comment;
        set => this.RaiseAndSetIfChanged(ref _comment, value);
    }

    public string NewTag
    {
        get => _newTag;
        set => this.RaiseAndSetIfChanged(ref _newTag, value);
    }

    public ObservableCollection<TagItemViewModel> TagItems { get; } = [];

    public ReactiveCommand<Window, Unit> OkCommand { get; }
    public ReactiveCommand<Window, Unit> CancelCommand { get; }
    public ReactiveCommand<Unit, Unit> ResolveLinkCommand { get; }

    /// <summary>編集内容を指定した Item に適用します</summary>
    public void ApplyTo(Item item)
    {
        item.DisplayName = DisplayName;
        item.FilePath = FilePath;
        item.Arguments = Arguments;
        item.WorkingDirectory = WorkingDirectory;
        item.Comment = Comment;

        var tags = TagItems
            .Where(t => t.IsChecked)
            .Select(t => t.Name)
            .ToList();

        if (!string.IsNullOrWhiteSpace(NewTag))
        {
            tags.Add(NewTag.Trim());
        }

        item.Tags = [.. tags.Distinct()];
    }

    private void OnOk(Window window)
    {
        IsConfirmed = true;
        OkPressed?.Invoke(this, EventArgs.Empty);
        window.Close();
    }

    private void OnCancel(Window window)
    {
        IsConfirmed = false;
        CancelPressed?.Invoke(this, EventArgs.Empty);
        window.Close();
    }

    private void OnResolveLink()
    {
        // TODO: .lnk ファイルを解析して FilePath, WorkingDirectory, Arguments を展開する
    }
}

public class TagItemViewModel(string name, bool isChecked = false) : ViewModelBase
{
    private bool _isChecked = isChecked;

    public string Name { get; } = name;

    public bool IsChecked
    {
        get => _isChecked;
        set => this.RaiseAndSetIfChanged(ref _isChecked, value);
    }
}