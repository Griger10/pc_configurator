using Microsoft.Extensions.DependencyInjection;
using PCConfigurator.UI.ViewModels;
using System.Windows;

namespace PCConfigurator.UI.Views;

public partial class LoginWindow : Window
{
    private readonly LoginViewModel _viewModel;

    public LoginWindow(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;

        // Когда авторизация прошла — открываем MainWindow и закрываем это окно
        _viewModel.LoginSucceeded += OnLoginSucceeded;
    }

    private void OnLoginSucceeded()
    {
        var mainWindow = App.Services.GetRequiredService<MainWindow>();

        // Обновить логин в сайдбаре (нужно при повторном входе после выхода)
        if (mainWindow.DataContext is MainViewModel mainVm)
            mainVm.RefreshUserInfo();

        mainWindow.Show();
        Hide(); // Hide, не Close — Singleton-окно нужно снова при следующем выходе
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = PasswordBox.Password;
    }
}
