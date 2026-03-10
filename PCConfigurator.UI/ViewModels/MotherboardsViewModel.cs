using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;

namespace PCConfigurator.UI.ViewModels;

public class MotherboardsViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;
    private readonly UserSession _session;

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

    public bool IsAdmin => _session.IsAdmin;

    private Motherboard? _selectedMotherboard;
    public Motherboard? SelectedMotherboard
    {
        get => _selectedMotherboard;
        set => SetProperty(ref _selectedMotherboard, value);
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

    private string _editSocket = "";
    public string EditSocket
    {
        get => _editSocket;
        set => SetProperty(ref _editSocket, value);
    }

    private string _editChipset = "";
    public string EditChipset
    {
        get => _editChipset;
        set => SetProperty(ref _editChipset, value);
    }

    private string _editRamType = "DDR5";
    public string EditRamType
    {
        get => _editRamType;
        set => SetProperty(ref _editRamType, value);
    }

    private int _editMaxRam = 128;
    public int EditMaxRam
    {
        get => _editMaxRam;
        set => SetProperty(ref _editMaxRam, value);
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

    public MotherboardsViewModel(PCConfiguratorContext db, UserSession session)
    {
        _db = db;
        _session = session;
        LoadCommand      = new AsyncRelayCommand(LoadInternalAsync);
        StartAddCommand  = new RelayCommand(StartAdd);
        StartEditCommand = new RelayCommand(StartEdit, () => SelectedMotherboard != null);
        SaveEditCommand  = new AsyncRelayCommand(SaveEditAsync);
        CancelEditCommand = new RelayCommand(() => IsEditPanelOpen = false);
        DeleteCommand    = new AsyncRelayCommand(DeleteAsync, () => SelectedMotherboard != null);
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

    private void StartAdd()
    {
        IsAddMode = true;
        EditManufacturer = AvailableManufacturers.FirstOrDefault();
        EditModel = "";
        EditSocket = "";
        EditChipset = "";
        EditRamType = "DDR5";
        EditMaxRam = 128;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private void StartEdit()
    {
        if (SelectedMotherboard is null) return;
        IsAddMode = false;
        EditManufacturer = AvailableManufacturers.FirstOrDefault(m => m.ManufacturerId == SelectedMotherboard.ManufacturerId);
        EditModel = SelectedMotherboard.Model;
        EditSocket = SelectedMotherboard.Socket;
        EditChipset = SelectedMotherboard.Chipset;
        EditRamType = SelectedMotherboard.Ramtype;
        EditMaxRam = SelectedMotherboard.MaxRam;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private async Task SaveEditAsync()
    {
        if (EditManufacturer is null || string.IsNullOrWhiteSpace(EditModel) ||
            string.IsNullOrWhiteSpace(EditSocket) || string.IsNullOrWhiteSpace(EditChipset) ||
            string.IsNullOrWhiteSpace(EditRamType))
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
                _db.Motherboards.Add(new Motherboard
                {
                    ManufacturerId = EditManufacturer.ManufacturerId,
                    Model = EditModel.Trim(),
                    Socket = EditSocket.Trim(),
                    Chipset = EditChipset.Trim(),
                    Ramtype = EditRamType.Trim(),
                    MaxRam = EditMaxRam
                });
            }
            else
            {
                var item = await _db.Motherboards.FindAsync(SelectedMotherboard!.MotherboardId);
                if (item != null)
                {
                    item.ManufacturerId = EditManufacturer.ManufacturerId;
                    item.Model = EditModel.Trim();
                    item.Socket = EditSocket.Trim();
                    item.Chipset = EditChipset.Trim();
                    item.Ramtype = EditRamType.Trim();
                    item.MaxRam = EditMaxRam;
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
        if (SelectedMotherboard is null) return;
        EditIsError = false;
        try
        {
            var item = await _db.Motherboards.FindAsync(SelectedMotherboard.MotherboardId);
            if (item != null) _db.Motherboards.Remove(item);
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
