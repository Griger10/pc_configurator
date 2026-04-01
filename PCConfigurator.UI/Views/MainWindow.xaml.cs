using Microsoft.Extensions.DependencyInjection;
using PCConfigurator.UI.ViewModels;
using System.Windows;

namespace PCConfigurator.UI.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _mainViewModel;

    public MainWindow(MainViewModel mainViewModel)
    {
        InitializeComponent();
        _mainViewModel = mainViewModel;
        DataContext = _mainViewModel;

        _mainViewModel.LogoutRequested += OnLogoutRequested;
    }

    private void OnLogoutRequested()
    {
        var loginWindow = App.Services.GetRequiredService<LoginWindow>();

        // Сброс состояния формы входа
        if (loginWindow.DataContext is LoginViewModel loginVm)
            loginVm.Reset();

        loginWindow.Show();
        Hide();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {

    }
}
