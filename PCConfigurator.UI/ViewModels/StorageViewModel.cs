using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class StorageViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    private ObservableCollection<Storage> _storages = new();
    public ObservableCollection<Storage> Storages
    {
        get => _storages;
        private set => SetProperty(ref _storages, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoadCommand { get; }

    public StorageViewModel(PCConfiguratorContext db)
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
            var data = await _db.Storages
                .Include(s => s.Manufacturer)
                .AsNoTracking()
                .OrderBy(s => s.Type)
                .ThenBy(s => s.Capacity)
                .ToListAsync();
            Storages = new ObservableCollection<Storage>(data);
        }
        finally
        {
            IsLoading = false;
        }
    }
}
