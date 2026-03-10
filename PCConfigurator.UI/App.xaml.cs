using System.Windows;
using Курсовой_Конфигуратор_ПК.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PCConfigurator.UI.Services;
using PCConfigurator.UI.ViewModels;
using PCConfigurator.UI.Views;

namespace PCConfigurator.UI;

public partial class App : Application
{
    private IServiceProvider _serviceProvider = null!;

    /// <summary>Статический доступ к DI-контейнеру (используется в LoginWindow codebehind)</summary>
    public static IServiceProvider Services { get; private set; } = null!;

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

        // Сессия текущего пользователя (Singleton — живёт весь сеанс)
        services.AddSingleton<UserSession>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ProcessorsViewModel>();
        services.AddTransient<MotherboardsViewModel>();
        services.AddTransient<RamViewModel>();
        services.AddTransient<GpusViewModel>();
        services.AddTransient<StorageViewModel>();
        services.AddTransient<ConfigurationsViewModel>();
        services.AddTransient<ConfiguratorViewModel>();
        services.AddSingleton<MainViewModel>();

        // Windows
        services.AddSingleton<LoginWindow>();
        services.AddSingleton<MainWindow>();

        _serviceProvider = services.BuildServiceProvider();
        Services = _serviceProvider;

        // Авто-применение миграций при старте: создаёт БД если не существует,
        // применяет все ещё не применённые миграции
        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PCConfiguratorContext>();
            db.Database.Migrate();
        }

        // Показываем окно входа; MainWindow откроется после успешной авторизации
        var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
        loginWindow.Show();
    }
}
