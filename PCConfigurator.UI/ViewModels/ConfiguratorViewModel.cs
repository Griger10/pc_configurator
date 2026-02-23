using System.Collections.ObjectModel;
using Курсовой_Конфигуратор_ПК.Data;
using Курсовой_Конфигуратор_ПК.Models;
using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;

namespace PCConfigurator.UI.ViewModels;

public class ConfiguratorViewModel : ViewModelBase, ILoadable
{
    private readonly PCConfiguratorContext _db;

    // Доступные компоненты (для ComboBox)
    public ObservableCollection<Processor> AvailableProcessors { get; } = new();
    public ObservableCollection<Motherboard> AvailableMotherboards { get; } = new();
    public ObservableCollection<Ram> AvailableRams { get; } = new();
    public ObservableCollection<Gpu> AvailableGpus { get; } = new();
    public ObservableCollection<Storage> AvailableStorages { get; } = new();

    // Выбранные компоненты
    private Processor? _selectedProcessor;
    public Processor? SelectedProcessor
    {
        get => _selectedProcessor;
        set => SetProperty(ref _selectedProcessor, value);
    }

    private Motherboard? _selectedMotherboard;
    public Motherboard? SelectedMotherboard
    {
        get => _selectedMotherboard;
        set => SetProperty(ref _selectedMotherboard, value);
    }

    private Gpu? _selectedGpu;
    public Gpu? SelectedGpu
    {
        get => _selectedGpu;
        set => SetProperty(ref _selectedGpu, value);
    }

    private Ram? _selectedRam;
    public Ram? SelectedRam
    {
        get => _selectedRam;
        set => SetProperty(ref _selectedRam, value);
    }

    private int _ramQuantity = 1;
    public int RamQuantity
    {
        get => _ramQuantity;
        set => SetProperty(ref _ramQuantity, value);
    }

    private Storage? _selectedStorage;
    public Storage? SelectedStorage
    {
        get => _selectedStorage;
        set => SetProperty(ref _selectedStorage, value);
    }

    private int _storageQuantity = 1;
    public int StorageQuantity
    {
        get => _storageQuantity;
        set => SetProperty(ref _storageQuantity, value);
    }

    // Метаданные конфигурации
    private string _configName = string.Empty;
    public string ConfigName
    {
        get => _configName;
        set => SetProperty(ref _configName, value);
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    private bool _isError;
    public bool IsError
    {
        get => _isError;
        private set => SetProperty(ref _isError, value);
    }

    public AsyncRelayCommand LoadCommand { get; }
    public AsyncRelayCommand SaveCommand { get; }
    public RelayCommand ResetCommand { get; }

    public ConfiguratorViewModel(PCConfiguratorContext db)
    {
        _db = db;
        LoadCommand = new AsyncRelayCommand(LoadInternalAsync);
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        ResetCommand = new RelayCommand(Reset);
    }

    public void Load() => LoadCommand.Execute(null);

    private async Task LoadInternalAsync()
    {
        IsError = false;
        StatusMessage = string.Empty;
        try
        {
            var processors  = await _db.Processors.Include(p => p.Manufacturer).AsNoTracking().OrderBy(p => p.Model).ToListAsync();
            var motherboards = await _db.Motherboards.Include(m => m.Manufacturer).AsNoTracking().OrderBy(m => m.Model).ToListAsync();
            var rams        = await _db.Rams.Include(r => r.Manufacturer).AsNoTracking().OrderBy(r => r.Model).ToListAsync();
            var gpus        = await _db.Gpus.Include(g => g.Manufacturer).AsNoTracking().OrderBy(g => g.Model).ToListAsync();
            var storages    = await _db.Storages.Include(s => s.Manufacturer).AsNoTracking().OrderBy(s => s.Model).ToListAsync();

            AvailableProcessors.Clear();
            foreach (var p in processors) AvailableProcessors.Add(p);

            AvailableMotherboards.Clear();
            foreach (var m in motherboards) AvailableMotherboards.Add(m);

            AvailableRams.Clear();
            foreach (var r in rams) AvailableRams.Add(r);

            AvailableGpus.Clear();
            foreach (var g in gpus) AvailableGpus.Add(g);

            AvailableStorages.Clear();
            foreach (var s in storages) AvailableStorages.Add(s);
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"Ошибка загрузки данных: {ex.Message}";
        }
    }

    private async Task SaveAsync()
    {
        IsError = false;
        StatusMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(ConfigName) || SelectedProcessor is null || SelectedMotherboard is null)
        {
            IsError = true;
            StatusMessage = "Заполните обязательные поля: название, процессор, материнская плата.";
            return;
        }

        if (SelectedProcessor.Socket != SelectedMotherboard.Socket)
        {
            IsError = true;
            StatusMessage = $"Несовместимые сокеты: процессор использует {SelectedProcessor.Socket}, " +
                            $"а материнская плата — {SelectedMotherboard.Socket}.";
            return;
        }

        if (SelectedRam is not null && SelectedRam.Type != SelectedMotherboard.Ramtype)
        {
            IsError = true;
            StatusMessage = $"Несовместимая оперативная память: планки {SelectedRam.Type}, " +
                            $"а материнская плата поддерживает {SelectedMotherboard.Ramtype}.";
            return;
        }

        try
        {
            var config = new Configuration
            {
                Name = ConfigName,
                ProcessorId = SelectedProcessor.ProcessorId,
                MotherboardId = SelectedMotherboard.MotherboardId,
                Gpuid = SelectedGpu?.Gpuid,
                CreatedDate = DateTime.Now
            };

            _db.Configurations.Add(config);
            await _db.SaveChangesAsync();

            if (SelectedRam is not null)
            {
                _db.ConfigurationRams.Add(new ConfigurationRam
                {
                    ConfigurationId = config.ConfigurationId,
                    Ramid = SelectedRam.Ramid,
                    Quantity = Math.Max(1, RamQuantity)
                });
            }

            if (SelectedStorage is not null)
            {
                _db.ConfigurationStorages.Add(new ConfigurationStorage
                {
                    ConfigurationId = config.ConfigurationId,
                    StorageId = SelectedStorage.StorageId,
                    Quantity = Math.Max(1, StorageQuantity)
                });
            }

            if (SelectedRam is not null || SelectedStorage is not null)
                await _db.SaveChangesAsync();

            StatusMessage = $"Конфигурация \"{ConfigName}\" успешно сохранена!";
            Reset();
        }
        catch (Exception ex)
        {
            IsError = true;
            StatusMessage = $"Ошибка при сохранении: {ex.Message}";
        }
    }

    private void Reset()
    {
        ConfigName = string.Empty;
        SelectedProcessor = null;
        SelectedMotherboard = null;
        SelectedGpu = null;
        SelectedRam = null;
        RamQuantity = 1;
        SelectedStorage = null;
        StorageQuantity = 1;
        IsError = false;
        StatusMessage = string.Empty;
    }
}
