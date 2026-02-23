using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class MotherboardsViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    private ObservableCollection<Motherboard> _motherboards = new();
    public ObservableCollection<Motherboard> Motherboards
    {
        get => _motherboards;
        private set => SetProperty(ref _motherboards, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoadCommand { get; }

    public MotherboardsViewModel(PCConfiguratorContext db)
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
            var data = await _db.Motherboards
                .Include(m => m.Manufacturer)
                .AsNoTracking()
                .OrderBy(m => m.Manufacturer!.Name)
                .ThenBy(m => m.Model)
                .ToListAsync();
            Motherboards = new ObservableCollection<Motherboard>(data);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
