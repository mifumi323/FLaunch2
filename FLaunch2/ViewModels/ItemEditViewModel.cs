using Avalonia.Controls;
using FLaunch2.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace FLaunch2.ViewModels;

internal static class ItemTypeExtensions
{
    public static readonly ItemType[] AllItemTypes = Enum.GetValues<ItemType>();
}

public class ItemEditViewModel : ViewModelBase
{
    public ItemEditViewModel()
    {
        OkCommand = ReactiveCommand.Create<Window>(OnOk);
        CancelCommand = ReactiveCommand.Create<Window>(OnCancel);
        ResolveLinkCommand = ReactiveCommand.Create(OnResolveLink);
    }

    public ItemEditViewModel(Item item, IEnumerable<string> allTags, bool isNew) : this()
    {
        IsNew = isNew;

        DisplayName = item.DisplayName;
        FilePath = item.FilePath;
        Arguments = item.Arguments;
        WorkingDirectory = item.WorkingDirectory;
        Comment = item.Comment;
        RunAsAdministrator = item.RunAsAdministrator;
        ItemType = item.ItemType;

        foreach (var tag in allTags)
        {
            TagItems.Add(new TagItemViewModel(tag, isChecked: item.Tags.Contains(tag)));
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
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string FilePath
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string Arguments
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string WorkingDirectory
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public string Comment
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public bool RunAsAdministrator
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public string NewTag
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    public ItemType ItemType
    {
        get; set => this.RaiseAndSetIfChanged(ref field, value);
    } = ItemType.NativeApp;

    public ObservableCollection<TagItemViewModel> TagItems { get; } = [];

    public ItemType[] ItemTypes => ItemTypeExtensions.AllItemTypes;

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
        item.RunAsAdministrator = RunAsAdministrator;
        item.ItemType = ItemType;

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
        if (!FilePath.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        try
        {
            var link = new Services.ShortcutLink(FilePath);
            FilePath = link.TargetPath;
            Arguments = link.Arguments;
            WorkingDirectory = link.WorkingDirectory;
            if (string.IsNullOrEmpty(Comment) && !string.IsNullOrEmpty(link.Description))
            {
                Comment = link.Description;
            }
        }
        catch (Exception)
        {
            // 解析失敗時は何もしない
        }
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