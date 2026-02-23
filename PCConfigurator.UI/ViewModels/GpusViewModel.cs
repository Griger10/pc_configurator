using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class GpusViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    private ObservableCollection<Gpu> _gpus = new();
    public ObservableCollection<Gpu> Gpus
    {
        get => _gpus;
        private set => SetProperty(ref _gpus, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoadCommand { get; }

    public GpusViewModel(PCConfiguratorContext db)
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
            var data = await _db.Gpus
                .Include(g => g.Manufacturer)
                .AsNoTracking()
                .OrderBy(g => g.Manufacturer!.Name)
                .ThenBy(g => g.Model)
                .ToListAsync();
            Gpus = new ObservableCollection<Gpu>(data);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
