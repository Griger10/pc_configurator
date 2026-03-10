using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;

namespace PCConfigurator.UI.ViewModels;

public class GpusViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;
    private readonly UserSession _session;

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

    public bool IsAdmin => _session.IsAdmin;

    private Gpu? _selectedGpu;
    public Gpu? SelectedGpu
    {
        get => _selectedGpu;
        set => SetProperty(ref _selectedGpu, value);
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

    private int _editMemory = 8;
    public int EditMemory
    {
        get => _editMemory;
        set => SetProperty(ref _editMemory, value);
    }

    private string _editMemoryType = "GDDR6";
    public string EditMemoryType
    {
        get => _editMemoryType;
        set => SetProperty(ref _editMemoryType, value);
    }

    private int _editPowerConsumption = 200;
    public int EditPowerConsumption
    {
        get => _editPowerConsumption;
        set => SetProperty(ref _editPowerConsumption, value);
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

    public GpusViewModel(PCConfiguratorContext db, UserSession session)
    {
        _db = db;
        _session = session;
        LoadCommand      = new AsyncRelayCommand(LoadInternalAsync);
        StartAddCommand  = new RelayCommand(StartAdd);
        StartEditCommand = new RelayCommand(StartEdit, () => SelectedGpu != null);
        SaveEditCommand  = new AsyncRelayCommand(SaveEditAsync);
        CancelEditCommand = new RelayCommand(() => IsEditPanelOpen = false);
        DeleteCommand    = new AsyncRelayCommand(DeleteAsync, () => SelectedGpu != null);
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

    private void StartAdd()
    {
        IsAddMode = true;
        EditManufacturer = AvailableManufacturers.FirstOrDefault();
        EditModel = "";
        EditMemory = 8;
        EditMemoryType = "GDDR6";
        EditPowerConsumption = 200;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private void StartEdit()
    {
        if (SelectedGpu is null) return;
        IsAddMode = false;
        EditManufacturer = AvailableManufacturers.FirstOrDefault(m => m.ManufacturerId == SelectedGpu.ManufacturerId);
        EditModel = SelectedGpu.Model;
        EditMemory = SelectedGpu.Memory;
        EditMemoryType = SelectedGpu.MemoryType;
        EditPowerConsumption = SelectedGpu.PowerConsumption;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private async Task SaveEditAsync()
    {
        if (EditManufacturer is null || string.IsNullOrWhiteSpace(EditModel) || string.IsNullOrWhiteSpace(EditMemoryType))
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
                _db.Gpus.Add(new Gpu
                {
                    ManufacturerId = EditManufacturer.ManufacturerId,
                    Model = EditModel.Trim(),
                    Memory = EditMemory,
                    MemoryType = EditMemoryType.Trim(),
                    PowerConsumption = EditPowerConsumption
                });
            }
            else
            {
                var item = await _db.Gpus.FindAsync(SelectedGpu!.Gpuid);
                if (item != null)
                {
                    item.ManufacturerId = EditManufacturer.ManufacturerId;
                    item.Model = EditModel.Trim();
                    item.Memory = EditMemory;
                    item.MemoryType = EditMemoryType.Trim();
                    item.PowerConsumption = EditPowerConsumption;
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
        if (SelectedGpu is null) return;
        EditIsError = false;
        try
        {
            var item = await _db.Gpus.FindAsync(SelectedGpu.Gpuid);
            if (item != null) _db.Gpus.Remove(item);
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
