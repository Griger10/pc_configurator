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
        mainWindow.Show();
        Close();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        _viewModel.Password = PasswordBox.Password;
    }
}
