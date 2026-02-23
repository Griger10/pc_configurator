using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class ProcessorsViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    private ObservableCollection<Processor> _processors = new();
    public ObservableCollection<Processor> Processors
    {
        get => _processors;
        private set => SetProperty(ref _processors, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoadCommand { get; }

    public ProcessorsViewModel(PCConfiguratorContext db)
    {
        _db = db;
        LoadCommand = new AsyncRelayCommand(LoadInternalAsync);
    }

    public void Load() => LoadCommand.Execute(null);

    private async Task LoadInternalAsync()
    {
        IsLoading = true;
        try
        {
            var data = await _db.Processors
                .Include(p => p.Manufacturer)
                .AsNoTracking()
                .OrderBy(p => p.Manufacturer!.Name)
                .ThenBy(p => p.Model)
                .ToListAsync();
            Processors = new ObservableCollection<Processor>(data);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
