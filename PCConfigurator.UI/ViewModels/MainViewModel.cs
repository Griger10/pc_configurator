using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;
using Курсовой_Конфигуратор_ПК.Data;
using Microsoft.EntityFrameworkCore;

namespace PCConfigurator.UI.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly ProcessorsViewModel _processorsVm;
    private readonly MotherboardsViewModel _motherboardsVm;
    private readonly RamViewModel _ramVm;
    private readonly GpusViewModel _gpusVm;
    private readonly StorageViewModel _storageVm;
    private readonly ConfigurationsViewModel _configurationsVm;
    private readonly ConfiguratorViewModel _configuratorVm;
    private readonly UserSession _session;
    private readonly IServiceScopeFactory _scopeFactory;

    private ViewModelBase _currentView;
    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    /// <summary>Логин текущего пользователя для отображения в сайдбаре</summary>
    public string CurrentUserLogin => _session.Login;

    /// <summary>Срабатывает при выходе — MainWindow подписывается и показывает LoginWindow</summary>
    public event Action? LogoutRequested;

    public RelayCommand NavigateProcessorsCommand { get; }
    public RelayCommand NavigateMotherboardsCommand { get; }
    public RelayCommand NavigateRamCommand { get; }
    public RelayCommand NavigateGpusCommand { get; }
    public RelayCommand NavigateStorageCommand { get; }
    public RelayCommand NavigateConfigurationsCommand { get; }
    public RelayCommand NavigateConfiguratorCommand { get; }
    public AsyncRelayCommand ExportToExcelCommand { get; }
    public RelayCommand LogoutCommand { get; }

    /// <summary>Обновить отображение логина в сайдбаре после повторного входа</summary>
    public void RefreshUserInfo() => OnPropertyChanged(nameof(CurrentUserLogin));

    public MainViewModel(
        ProcessorsViewModel processorsVm,
        MotherboardsViewModel motherboardsVm,
        RamViewModel ramVm,
        GpusViewModel gpusVm,
        StorageViewModel storageVm,
        ConfigurationsViewModel configurationsVm,
        ConfiguratorViewModel configuratorVm,
        UserSession session,
        IServiceScopeFactory scopeFactory)
    {
        _processorsVm     = processorsVm;
        _motherboardsVm   = motherboardsVm;
        _ramVm            = ramVm;
        _gpusVm           = gpusVm;
        _storageVm        = storageVm;
        _configurationsVm = configurationsVm;
        _configuratorVm   = configuratorVm;
        _session          = session;
        _scopeFactory     = scopeFactory;

        NavigateProcessorsCommand     = new RelayCommand(() => Navigate(_processorsVm));
        NavigateMotherboardsCommand   = new RelayCommand(() => Navigate(_motherboardsVm));
        NavigateRamCommand            = new RelayCommand(() => Navigate(_ramVm));
        NavigateGpusCommand           = new RelayCommand(() => Navigate(_gpusVm));
        NavigateStorageCommand        = new RelayCommand(() => Navigate(_storageVm));
        NavigateConfigurationsCommand = new RelayCommand(() => Navigate(_configurationsVm));
        NavigateConfiguratorCommand   = new RelayCommand(() => Navigate(_configuratorVm));
        ExportToExcelCommand          = new AsyncRelayCommand(ExportToExcelAsync);
        LogoutCommand                 = new RelayCommand(Logout);

        // Страница по умолчанию
        _currentView = _processorsVm;
        _processorsVm.Load();
    }

    private void Logout()
    {
        _session.Reset();
        OnPropertyChanged(nameof(CurrentUserLogin));
        LogoutRequested?.Invoke();
    }

    private void Navigate(ViewModelBase viewModel)
    {
        CurrentView = viewModel;
        if (viewModel is ILoadable loadable)
            loadable.Load();
    }

    private async Task ExportToExcelAsync()
    {
        var dialog = new SaveFileDialog
        {
            Title = "Экспорт базы данных в Excel",
            Filter = "Excel Workbook|*.xlsx",
            FileName = $"PCConfigurator_Export_{DateTime.Now:yyyyMMdd_HHmm}.xlsx"
        };

        if (dialog.ShowDialog() != true) return;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PCConfiguratorContext>();

        using var wb = new XLWorkbook();

        // ── Лист 1: Пользователи (без пароля) ──────────────────
        var wsUsers = wb.Worksheets.Add("Пользователи");
        wsUsers.Cell(1, 1).Value = "ID";
        wsUsers.Cell(1, 2).Value = "Логин";
        wsUsers.Cell(1, 3).Value = "Роль";
        wsUsers.Row(1).Style.Font.Bold = true;
        var users = await db.Users.AsNoTracking().OrderBy(u => u.Login).ToListAsync();
        for (int i = 0; i < users.Count; i++)
        {
            wsUsers.Cell(i + 2, 1).Value = users[i].UserId;
            wsUsers.Cell(i + 2, 2).Value = users[i].Login;
            wsUsers.Cell(i + 2, 3).Value = users[i].Role;
        }
        wsUsers.Columns().AdjustToContents();

        // ── Лист 2: Конфигурации ────────────────────────────────
        var wsConf = wb.Worksheets.Add("Конфигурации");
        wsConf.Cell(1, 1).Value = "ID";
        wsConf.Cell(1, 2).Value = "Название";
        wsConf.Cell(1, 3).Value = "Пользователь";
        wsConf.Cell(1, 4).Value = "Процессор";
        wsConf.Cell(1, 5).Value = "Материнская плата";
        wsConf.Cell(1, 6).Value = "Видеокарта";
        wsConf.Cell(1, 7).Value = "Оперативная память";
        wsConf.Cell(1, 8).Value = "Накопитель";
        wsConf.Cell(1, 9).Value = "Дата создания";
        wsConf.Row(1).Style.Font.Bold = true;
        var configs = await db.Configurations
            .Include(c => c.Processor)
            .Include(c => c.Motherboard)
            .Include(c => c.Gpu)
            .Include(c => c.User)
            .Include(c => c.ConfigurationRams).ThenInclude(cr => cr.Ram)
            .Include(c => c.ConfigurationStorages).ThenInclude(cs => cs.Storage)
            .AsNoTracking().OrderByDescending(c => c.CreatedDate).ToListAsync();
        for (int i = 0; i < configs.Count; i++)
        {
            var c = configs[i];
            wsConf.Cell(i + 2, 1).Value = c.ConfigurationId;
            wsConf.Cell(i + 2, 2).Value = c.Name;
            wsConf.Cell(i + 2, 3).Value = c.User?.Login ?? "—";
            wsConf.Cell(i + 2, 4).Value = c.Processor.Model;
            wsConf.Cell(i + 2, 5).Value = c.Motherboard.Model;
            wsConf.Cell(i + 2, 6).Value = c.Gpu?.Model ?? "—";
            wsConf.Cell(i + 2, 7).Value = c.ConfigurationRams.Any()
                ? string.Join(", ", c.ConfigurationRams.Select(cr => $"{cr.Ram.Model} x{cr.Quantity}")) : "—";
            wsConf.Cell(i + 2, 8).Value = c.ConfigurationStorages.Any()
                ? string.Join(", ", c.ConfigurationStorages.Select(cs => $"{cs.Storage.Model} x{cs.Quantity}")) : "—";
            wsConf.Cell(i + 2, 9).Value = c.CreatedDate.ToString("dd.MM.yyyy");
        }
        wsConf.Columns().AdjustToContents();

        // ── Лист 3: Процессоры ─────────────────────────────────
        var wsProc = wb.Worksheets.Add("Процессоры");
        wsProc.Cell(1, 1).Value = "ID";
        wsProc.Cell(1, 2).Value = "Производитель";
        wsProc.Cell(1, 3).Value = "Модель";
        wsProc.Cell(1, 4).Value = "Сокет";
        wsProc.Cell(1, 5).Value = "Ядра";
        wsProc.Cell(1, 6).Value = "Частота ГГц";
        wsProc.Row(1).Style.Font.Bold = true;
        var procs = await db.Processors.Include(p => p.Manufacturer).AsNoTracking().OrderBy(p => p.Model).ToListAsync();
        for (int i = 0; i < procs.Count; i++)
        {
            wsProc.Cell(i + 2, 1).Value = procs[i].ProcessorId;
            wsProc.Cell(i + 2, 2).Value = procs[i].Manufacturer.Name;
            wsProc.Cell(i + 2, 3).Value = procs[i].Model;
            wsProc.Cell(i + 2, 4).Value = procs[i].Socket;
            wsProc.Cell(i + 2, 5).Value = procs[i].Cores;
            wsProc.Cell(i + 2, 6).Value = (double)procs[i].Frequency;
        }
        wsProc.Columns().AdjustToContents();

        // ── Лист 4: Материнские платы ──────────────────────────
        var wsMb = wb.Worksheets.Add("Материнские платы");
        wsMb.Cell(1, 1).Value = "ID";
        wsMb.Cell(1, 2).Value = "Производитель";
        wsMb.Cell(1, 3).Value = "Модель";
        wsMb.Cell(1, 4).Value = "Сокет";
        wsMb.Cell(1, 5).Value = "Чипсет";
        wsMb.Cell(1, 6).Value = "Тип RAM";
        wsMb.Cell(1, 7).Value = "Макс. RAM ГБ";
        wsMb.Row(1).Style.Font.Bold = true;
        var mbs = await db.Motherboards.Include(m => m.Manufacturer).AsNoTracking().OrderBy(m => m.Model).ToListAsync();
        for (int i = 0; i < mbs.Count; i++)
        {
            wsMb.Cell(i + 2, 1).Value = mbs[i].MotherboardId;
            wsMb.Cell(i + 2, 2).Value = mbs[i].Manufacturer.Name;
            wsMb.Cell(i + 2, 3).Value = mbs[i].Model;
            wsMb.Cell(i + 2, 4).Value = mbs[i].Socket;
            wsMb.Cell(i + 2, 5).Value = mbs[i].Chipset;
            wsMb.Cell(i + 2, 6).Value = mbs[i].Ramtype;
            wsMb.Cell(i + 2, 7).Value = mbs[i].MaxRam;
        }
        wsMb.Columns().AdjustToContents();

        // ── Лист 5: Оперативная память ─────────────────────────
        var wsRam = wb.Worksheets.Add("Оперативная память");
        wsRam.Cell(1, 1).Value = "ID";
        wsRam.Cell(1, 2).Value = "Производитель";
        wsRam.Cell(1, 3).Value = "Модель";
        wsRam.Cell(1, 4).Value = "Тип";
        wsRam.Cell(1, 5).Value = "Объём ГБ";
        wsRam.Cell(1, 6).Value = "Частота МГц";
        wsRam.Row(1).Style.Font.Bold = true;
        var rams = await db.Rams.Include(r => r.Manufacturer).AsNoTracking().OrderBy(r => r.Model).ToListAsync();
        for (int i = 0; i < rams.Count; i++)
        {
            wsRam.Cell(i + 2, 1).Value = rams[i].Ramid;
            wsRam.Cell(i + 2, 2).Value = rams[i].Manufacturer.Name;
            wsRam.Cell(i + 2, 3).Value = rams[i].Model;
            wsRam.Cell(i + 2, 4).Value = rams[i].Type;
            wsRam.Cell(i + 2, 5).Value = rams[i].Capacity;
            wsRam.Cell(i + 2, 6).Value = rams[i].Frequency;
        }
        wsRam.Columns().AdjustToContents();

        // ── Лист 6: Видеокарты ─────────────────────────────────
        var wsGpu = wb.Worksheets.Add("Видеокарты");
        wsGpu.Cell(1, 1).Value = "ID";
        wsGpu.Cell(1, 2).Value = "Производитель";
        wsGpu.Cell(1, 3).Value = "Модель";
        wsGpu.Cell(1, 4).Value = "Память ГБ";
        wsGpu.Cell(1, 5).Value = "Тип памяти";
        wsGpu.Cell(1, 6).Value = "TDP Вт";
        wsGpu.Row(1).Style.Font.Bold = true;
        var gpus = await db.Gpus.Include(g => g.Manufacturer).AsNoTracking().OrderBy(g => g.Model).ToListAsync();
        for (int i = 0; i < gpus.Count; i++)
        {
            wsGpu.Cell(i + 2, 1).Value = gpus[i].Gpuid;
            wsGpu.Cell(i + 2, 2).Value = gpus[i].Manufacturer.Name;
            wsGpu.Cell(i + 2, 3).Value = gpus[i].Model;
            wsGpu.Cell(i + 2, 4).Value = gpus[i].Memory;
            wsGpu.Cell(i + 2, 5).Value = gpus[i].MemoryType;
            wsGpu.Cell(i + 2, 6).Value = gpus[i].PowerConsumption;
        }
        wsGpu.Columns().AdjustToContents();

        // ── Лист 7: Накопители ─────────────────────────────────
        var wsStorage = wb.Worksheets.Add("Накопители");
        wsStorage.Cell(1, 1).Value = "ID";
        wsStorage.Cell(1, 2).Value = "Производитель";
        wsStorage.Cell(1, 3).Value = "Модель";
        wsStorage.Cell(1, 4).Value = "Тип";
        wsStorage.Cell(1, 5).Value = "Объём ГБ";
        wsStorage.Cell(1, 6).Value = "Интерфейс";
        wsStorage.Row(1).Style.Font.Bold = true;
        var storages = await db.Storages.Include(s => s.Manufacturer).AsNoTracking().OrderBy(s => s.Model).ToListAsync();
        for (int i = 0; i < storages.Count; i++)
        {
            wsStorage.Cell(i + 2, 1).Value = storages[i].StorageId;
            wsStorage.Cell(i + 2, 2).Value = storages[i].Manufacturer.Name;
            wsStorage.Cell(i + 2, 3).Value = storages[i].Model;
            wsStorage.Cell(i + 2, 4).Value = storages[i].Type;
            wsStorage.Cell(i + 2, 5).Value = storages[i].Capacity;
            wsStorage.Cell(i + 2, 6).Value = storages[i].Interface;
        }
        wsStorage.Columns().AdjustToContents();

        // ── Лист 8: Производители ──────────────────────────────
        var wsMfr = wb.Worksheets.Add("Производители");
        wsMfr.Cell(1, 1).Value = "ID";
        wsMfr.Cell(1, 2).Value = "Название";
        wsMfr.Cell(1, 3).Value = "Страна";
        wsMfr.Cell(1, 4).Value = "Сайт";
        wsMfr.Row(1).Style.Font.Bold = true;
        var manufacturers = await db.Manufacturers.AsNoTracking().OrderBy(m => m.Name).ToListAsync();
        for (int i = 0; i < manufacturers.Count; i++)
        {
            wsMfr.Cell(i + 2, 1).Value = manufacturers[i].ManufacturerId;
            wsMfr.Cell(i + 2, 2).Value = manufacturers[i].Name;
            wsMfr.Cell(i + 2, 3).Value = manufacturers[i].Country ?? "—";
            wsMfr.Cell(i + 2, 4).Value = manufacturers[i].Website ?? "—";
        }
        wsMfr.Columns().AdjustToContents();

        wb.SaveAs(dialog.FileName);
    }
}
