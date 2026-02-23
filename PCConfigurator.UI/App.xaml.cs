using System.Windows;
using Курсовой_Конфигуратор_ПК.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PCConfigurator.UI.ViewModels;
using PCConfigurator.UI.Views;

namespace PCConfigurator.UI;

public partial class App : Application
{
    private IServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        const string connectionString =
            "Server=DESKTOP-62AA05E\\SQLEXPRESS;" +
            "Database=PCConfigurator;" +
            "Trusted_Connection=True;" +
            "TrustServerCertificate=True;";

        // DbContext — Transient: каждый ViewModel получает свой контекст
        services.AddDbContext<PCConfiguratorContext>(
            options => options.UseSqlServer(connectionString),
            ServiceLifetime.Transient);

        // ViewModels
        services.AddTransient<ProcessorsViewModel>();
        services.AddTransient<MotherboardsViewModel>();
        services.AddTransient<RamViewModel>();
        services.AddTransient<GpusViewModel>();
        services.AddTransient<StorageViewModel>();
        services.AddTransient<ConfigurationsViewModel>();
        services.AddTransient<ConfiguratorViewModel>();
        services.AddSingleton<MainViewModel>();

        // MainWindow
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
