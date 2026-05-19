using FLaunch2.Models;
using FLaunch2.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace FLaunch2.ViewModels;

public class ImportViewModel : ViewModelBase
{
    public ImportViewModel(IEnumerable<Item> items, IIconExtractor iconExtractor)
    {
        Items = new ObservableCollection<ImportItemViewModel>(
            items.Select(i => new ImportItemViewModel(i, iconExtractor)));
        SelectAllCommand = ReactiveCommand.Create(SelectAll);
        DeselectAllCommand = ReactiveCommand.Create(DeselectAll);
    }

    public ObservableCollection<ImportItemViewModel> Items { get; }

    public ReactiveCommand<Unit, Unit> SelectAllCommand { get; }
    public ReactiveCommand<Unit, Unit> DeselectAllCommand { get; }

    public IEnumerable<Item> SelectedItems =>
        Items.Where(i => i.IsSelected).Select(i => i.Item);

    private void SelectAll()
    {
        foreach (var item in Items) item.IsSelected = true;
    }

    private void DeselectAll()
    {
        foreach (var item in Items) item.IsSelected = false;
    }
}
