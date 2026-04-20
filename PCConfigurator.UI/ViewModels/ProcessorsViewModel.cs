using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;

namespace PCConfigurator.UI.ViewModels;

public class ProcessorsViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;
    private readonly UserSession _session;

    private ObservableCollection<Processor> _processors = new();
    public ObservableCollection<Processor> Processors
    {
        get => _processors;
        private set => SetProperty(ref _processors, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public bool IsAdmin => _session.IsAdmin;

    private Processor? _selectedProcessor;
    public Processor? SelectedProcessor
    {
        get => _selectedProcessor;
        set => SetProperty(ref _selectedProcessor, value);
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

    private int _editCores = 4;
    public int EditCores
    {
        get => _editCores;
        set => SetProperty(ref _editCores, value);
    }

    private decimal _editFrequency = 3.0m;
    public decimal EditFrequency
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

    public ProcessorsViewModel(PCConfiguratorContext db, UserSession session)
    {
        _db = db;
        _session = session;
        LoadCommand      = new AsyncRelayCommand(LoadInternalAsync);
        StartAddCommand  = new RelayCommand(StartAdd);
        StartEditCommand = new RelayCommand(StartEdit, () => SelectedProcessor != null);
        SaveEditCommand  = new AsyncRelayCommand(SaveEditAsync);
        CancelEditCommand = new RelayCommand(() => IsEditPanelOpen = false);
        DeleteCommand    = new AsyncRelayCommand(DeleteAsync, () => SelectedProcessor != null);
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

            var data = await _db.Processors
                .Include(p => p.Manufacturer)
                .AsNoTracking()
                .OrderBy(p => p.Manufacturer!.Name)
                .ThenBy(p => p.Model)
                .ToListAsync();
            Processors = new ObservableCollection<Processor>(data);
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
        EditCores = 4;
        EditFrequency = 3.0m;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private void StartEdit()
    {
        if (SelectedProcessor is null) return;
        IsAddMode = false;
        EditManufacturer = AvailableManufacturers.FirstOrDefault(m => m.ManufacturerId == SelectedProcessor.ManufacturerId);
        EditModel = SelectedProcessor.Model;
        EditSocket = SelectedProcessor.Socket;
        EditCores = SelectedProcessor.Cores;
        EditFrequency = SelectedProcessor.Frequency;
        EditStatus = "";
        EditIsError = false;
        IsEditPanelOpen = true;
    }

    private async Task SaveEditAsync()
    {
        if (EditManufacturer is null || string.IsNullOrWhiteSpace(EditModel) || string.IsNullOrWhiteSpace(EditSocket))
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
                _db.Processors.Add(new Processor
                {
                    ManufacturerId = EditManufacturer.ManufacturerId,
                    Model = EditModel.Trim(),
                    Socket = EditSocket.Trim(),
                    Cores = EditCores,
                    Frequency = EditFrequency
                });
            }
            else
            {
                var item = await _db.Processors.FindAsync(SelectedProcessor!.ProcessorId);
                if (item != null)
                {
                    item.ManufacturerId = EditManufacturer.ManufacturerId;
                    item.Model = EditModel.Trim();
                    item.Socket = EditSocket.Trim();
                    item.Cores = EditCores;
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
        if (SelectedProcessor is null) return;
        EditIsError = false;
        try
        {
            var item = await _db.Processors.FindAsync(SelectedProcessor.ProcessorId);
            if (item != null) _db.Processors.Remove(item);
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
