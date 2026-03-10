using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;

namespace PCConfigurator.UI.ViewModels;

public class RamViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;
    private readonly UserSession _session;

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

    public bool IsAdmin => _session.IsAdmin;

    private Ram? _selectedRam;
    public Ram? SelectedRam
    {
        get => _selectedRam;
        set => SetProperty(ref _selectedRam, value);
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

    private string _editType = "DDR5";
    public string EditType
    {
        get => _editType;
        set => SetProperty(ref _editType, value);
    }

    private int _editCapacity = 16;
    public int EditCapacity
    {
        get => _editCapacity;
        set => SetProperty(ref _editCapacity, value);
    }

    private int _editFrequency = 4800;
    public int EditFrequency
    {
        get => _editFrequency;
        set => SetProperty(ref _editFrequency, value);
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

    public RamViewModel(PCConfiguratorContext db, UserSession session)
    {
        _db = db;
        _session = session;
        LoadCommand      = new AsyncRelayCommand(LoadInternalAsync);
        StartAddCommand  = new RelayCommand(StartAdd);
        StartEditCommand = new RelayCommand(StartEdit, () => SelectedRam != null);
        SaveEditCommand  = new AsyncRelayCommand(SaveEditAsync);
        CancelEditCommand = new RelayCommand(() => IsEditPanelOpen = false);
        DeleteCommand    = new AsyncRelayCommand(DeleteAsync, () => SelectedRam != null);
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

    private void StartAdd()
    {
        IsAddMode = true;
        EditManufacturer = AvailableManufacturers.FirstOrDefault();
        EditModel = "";
        EditType = "DDR5";
        EditCapacity = 16;
        EditFrequency = 4800;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private void StartEdit()
    {
        if (SelectedRam is null) return;
        IsAddMode = false;
        EditManufacturer = AvailableManufacturers.FirstOrDefault(m => m.ManufacturerId == SelectedRam.ManufacturerId);
        EditModel = SelectedRam.Model;
        EditType = SelectedRam.Type;
        EditCapacity = SelectedRam.Capacity;
        EditFrequency = SelectedRam.Frequency;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private async Task SaveEditAsync()
    {
        if (EditManufacturer is null || string.IsNullOrWhiteSpace(EditModel) || string.IsNullOrWhiteSpace(EditType))
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
                _db.Rams.Add(new Ram
                {
                    ManufacturerId = EditManufacturer.ManufacturerId,
                    Model = EditModel.Trim(),
                    Type = EditType.Trim(),
                    Capacity = EditCapacity,
                    Frequency = EditFrequency
                });
            }
            else
            {
                var item = await _db.Rams.FindAsync(SelectedRam!.Ramid);
                if (item != null)
                {
                    item.ManufacturerId = EditManufacturer.ManufacturerId;
                    item.Model = EditModel.Trim();
                    item.Type = EditType.Trim();
                    item.Capacity = EditCapacity;
                    item.Frequency = EditFrequency;
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
        if (SelectedRam is null) return;
        EditIsError = false;
        try
        {
            var item = await _db.Rams.FindAsync(SelectedRam.Ramid);
            if (item != null) _db.Rams.Remove(item);
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
