using PCConfigurator.UI.MVVM;

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

    private ViewModelBase _currentView;
    public ViewModelBase CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }

    public RelayCommand NavigateProcessorsCommand { get; }
    public RelayCommand NavigateMotherboardsCommand { get; }
    public RelayCommand NavigateRamCommand { get; }
    public RelayCommand NavigateGpusCommand { get; }
    public RelayCommand NavigateStorageCommand { get; }
    public RelayCommand NavigateConfigurationsCommand { get; }
    public RelayCommand NavigateConfiguratorCommand { get; }

    public MainViewModel(
        ProcessorsViewModel processorsVm,
        MotherboardsViewModel motherboardsVm,
        RamViewModel ramVm,
        GpusViewModel gpusVm,
        StorageViewModel storageVm,
        ConfigurationsViewModel configurationsVm,
        ConfiguratorViewModel configuratorVm)
    {
        _processorsVm     = processorsVm;
        _motherboardsVm   = motherboardsVm;
        _ramVm            = ramVm;
        _gpusVm           = gpusVm;
        _storageVm        = storageVm;
        _configurationsVm = configurationsVm;
        _configuratorVm   = configuratorVm;

        NavigateProcessorsCommand     = new RelayCommand(() => Navigate(_processorsVm));
        NavigateMotherboardsCommand   = new RelayCommand(() => Navigate(_motherboardsVm));
        NavigateRamCommand            = new RelayCommand(() => Navigate(_ramVm));
        NavigateGpusCommand           = new RelayCommand(() => Navigate(_gpusVm));
        NavigateStorageCommand        = new RelayCommand(() => Navigate(_storageVm));
        NavigateConfigurationsCommand = new RelayCommand(() => Navigate(_configurationsVm));
        NavigateConfiguratorCommand   = new RelayCommand(() => Navigate(_configuratorVm));

        // Страница по умолчанию
        _currentView = _processorsVm;
        _processorsVm.Load();
    }

    private void Navigate(ViewModelBase viewModel)
    {
        CurrentView = viewModel;
        if (viewModel is ILoadable loadable)
            loadable.Load();
    }
}
