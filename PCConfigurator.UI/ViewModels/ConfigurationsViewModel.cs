using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class ConfigurationsViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    private ObservableCollection<Configuration> _configurations = new();
    public ObservableCollection<Configuration> Configurations
    {
        get => _configurations;
        private set => SetProperty(ref _configurations, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoadCommand { get; }

    public ConfigurationsViewModel(PCConfiguratorContext db)
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
            var data = await _db.Configurations
                .Include(c => c.Processor)
                .Include(c => c.Motherboard)
                .Include(c => c.Gpu)
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
            Configurations = new ObservableCollection<Configuration>(data);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
