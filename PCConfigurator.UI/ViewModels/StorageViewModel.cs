using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;

namespace PCConfigurator.UI.ViewModels;

public class StorageViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;
    private readonly UserSession _session;

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

    public bool IsAdmin => _session.IsAdmin;

    private Storage? _selectedStorage;
    public Storage? SelectedStorage
    {
        get => _selectedStorage;
        set => SetProperty(ref _selectedStorage, value);
    }

    public ObservableCollection<Manufacturer> AvailableManufacturers { get; } = new();

    private bool _isEditPanelOpen;
    public bool IsEditPanelOpen
    {
        get => _isEditPanelOpen;
        set => SetProperty(ref _isEditPanelOpen, value);
    }

    private bool _isAddMode;
    public bool IsAddMode
    {
        get => _isAddMode;
        set => SetProperty(ref _isAddMode, value);
    }

    private Manufacturer? _editManufacturer;
    public Manufacturer? EditManufacturer
    {
        get => _editManufacturer;
        set => SetProperty(ref _editManufacturer, value);
    }

    private string _editModel = "";
    public string EditModel
    {
        get => _editModel;
        set => SetProperty(ref _editModel, value);
    }

    private string _editType = "SSD";
    public string EditType
    {
        get => _editType;
        set => SetProperty(ref _editType, value);
    }

    private int _editCapacity = 1000;
    public int EditCapacity
    {
        get => _editCapacity;
        set => SetProperty(ref _editCapacity, value);
    }

    private string _editInterface = "NVMe";
    public string EditInterface
    {
        get => _editInterface;
        set => SetProperty(ref _editInterface, value);
    }

    private string _editStatus = "";
    public string EditStatus
    {
        get => _editStatus;
        set => SetProperty(ref _editStatus, value);
    }

    private bool _editIsError;
    public bool EditIsError
    {
        get => _editIsError;
        set => SetProperty(ref _editIsError, value);
    }

    public AsyncRelayCommand LoadCommand { get; }
    public RelayCommand StartAddCommand { get; }
    public RelayCommand StartEditCommand { get; }
    public AsyncRelayCommand SaveEditCommand { get; }
    public RelayCommand CancelEditCommand { get; }
    public AsyncRelayCommand DeleteCommand { get; }

    public StorageViewModel(PCConfiguratorContext db, UserSession session)
    {
        _db = db;
        _session = session;
        LoadCommand      = new AsyncRelayCommand(LoadInternalAsync);
        StartAddCommand  = new RelayCommand(StartAdd);
        StartEditCommand = new RelayCommand(StartEdit, () => SelectedStorage != null);
        SaveEditCommand  = new AsyncRelayCommand(SaveEditAsync);
        CancelEditCommand = new RelayCommand(() => IsEditPanelOpen = false);
        DeleteCommand    = new AsyncRelayCommand(DeleteAsync, () => SelectedStorage != null);
    }

    public void Load() => LoadCommand.Execute(null);

    private async Task LoadInternalAsync()
    {
        IsLoading = true;
        try
        {
            var manufacturers = await _db.Manufacturers.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
            AvailableManufacturers.Clear();
            foreach (var m in manufacturers) AvailableManufacturers.Add(m);

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

    private void StartAdd()
    {
        IsAddMode = true;
        EditManufacturer = AvailableManufacturers.FirstOrDefault();
        EditModel = "";
        EditType = "SSD";
        EditCapacity = 1000;
        EditInterface = "NVMe";
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private void StartEdit()
    {
        if (SelectedStorage is null) return;
        IsAddMode = false;
        EditManufacturer = AvailableManufacturers.FirstOrDefault(m => m.ManufacturerId == SelectedStorage.ManufacturerId);
        EditModel = SelectedStorage.Model;
        EditType = SelectedStorage.Type;
        EditCapacity = SelectedStorage.Capacity;
        EditInterface = SelectedStorage.Interface;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private async Task SaveEditAsync()
    {
        if (EditManufacturer is null || string.IsNullOrWhiteSpace(EditModel) ||
            string.IsNullOrWhiteSpace(EditType) || string.IsNullOrWhiteSpace(EditInterface))
        {
            EditStatus = "Заполните все поля.";
            EditIsError = true;
            return;
        }

        EditIsError = false;
        try
        {
            if (IsAddMode)
            {
                _db.Storages.Add(new Storage
                {
                    ManufacturerId = EditManufacturer.ManufacturerId,
                    Model = EditModel.Trim(),
                    Type = EditType.Trim(),
                    Capacity = EditCapacity,
                    Interface = EditInterface.Trim()
                });
            }
            else
            {
                var item = await _db.Storages.FindAsync(SelectedStorage!.StorageId);
                if (item != null)
                {
                    item.ManufacturerId = EditManufacturer.ManufacturerId;
                    item.Model = EditModel.Trim();
                    item.Type = EditType.Trim();
                    item.Capacity = EditCapacity;
                    item.Interface = EditInterface.Trim();
                }
            }

            await _db.SaveChangesAsync();
            IsEditPanelOpen = false;
            EditStatus = "";
            await LoadInternalAsync();
        }
        catch (Exception ex)
        {
            EditStatus = $"Ошибка: {ex.InnerException?.Message ?? ex.Message}";
            EditIsError = true;
        }
    }

    private async Task DeleteAsync()
    {
        if (SelectedStorage is null) return;
        EditIsError = false;
        try
        {
            var item = await _db.Storages.FindAsync(SelectedStorage.StorageId);
            if (item != null) _db.Storages.Remove(item);
            await _db.SaveChangesAsync();
            await LoadInternalAsync();
            EditStatus = "";
        }
        catch (Exception ex)
        {
            EditStatus = $"Нельзя удалить: {ex.InnerException?.Message ?? ex.Message}";
            EditIsError = true;
        }
    }
}
