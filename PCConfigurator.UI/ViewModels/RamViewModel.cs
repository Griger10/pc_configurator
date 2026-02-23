using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class RamViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    private ObservableCollection<Ram> _rams = new();
    public ObservableCollection<Ram> Rams
    {
        get => _rams;
        private set => SetProperty(ref _rams, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoadCommand { get; }

    public RamViewModel(PCConfiguratorContext db)
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
            var data = await _db.Rams
                .Include(r => r.Manufacturer)
                .AsNoTracking()
                .OrderBy(r => r.Type)
                .ThenBy(r => r.Capacity)
                .ToListAsync();
            Rams = new ObservableCollection<Ram>(data);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
