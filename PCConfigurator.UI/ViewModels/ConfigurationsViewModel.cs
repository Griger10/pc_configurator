using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class ConfigurationRow
{
    public int    ConfigurationId { get; init; }
    public string Name           { get; init; } = string.Empty;
    public string Processor      { get; init; } = string.Empty;
    public string Motherboard    { get; init; } = string.Empty;
    public string Gpu            { get; init; } = string.Empty;
    public string Ram            { get; init; } = string.Empty;
    public string Storage        { get; init; } = string.Empty;
    public string CreatedDate    { get; init; } = string.Empty;
}

public class ConfigurationsViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    private ObservableCollection<ConfigurationRow> _configurations = new();
    public ObservableCollection<ConfigurationRow> Configurations
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
                .Include(c => c.ConfigurationRams).ThenInclude(cr => cr.Ram)
                .Include(c => c.ConfigurationStorages).ThenInclude(cs => cs.Storage)
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            var rows = data.Select(c => new ConfigurationRow
            {
                ConfigurationId = c.ConfigurationId,
                Name        = c.Name,
                Processor   = c.Processor.Model,
                Motherboard = c.Motherboard.Model,
                Gpu         = c.Gpu?.Model ?? "—",
                Ram         = c.ConfigurationRams.Any()
                                  ? string.Join(", ", c.ConfigurationRams
                                        .Select(cr => $"{cr.Ram.Model} x{cr.Quantity}"))
                                  : "—",
                Storage     = c.ConfigurationStorages.Any()
                                  ? string.Join(", ", c.ConfigurationStorages
                                        .Select(cs => $"{cs.Storage.Model} x{cs.Quantity}"))
                                  : "—",
                CreatedDate = c.CreatedDate.ToString("dd.MM.yyyy")
            });

            Configurations = new ObservableCollection<ConfigurationRow>(rows);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
