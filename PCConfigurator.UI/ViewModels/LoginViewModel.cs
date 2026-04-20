using Microsoft.EntityFrameworkCore;
using PCConfigurator.UI.MVVM;
using PCConfigurator.UI.Services;
using Курсовой_Конфигуратор_ПК.Data;

namespace PCConfigurator.UI.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly PCConfiguratorContext _db;
    private readonly UserSession _session;

    private string _login = "";
    private string _password = "";
    private string _errorMessage = "";
    private bool _hasError;
    private bool _isLoading;

    public string Login
    {
        get => _login;
        set => SetProperty(ref _login, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    public bool HasError
    {
        get => _hasError;
        set => SetProperty(ref _hasError, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public AsyncRelayCommand LoginCommand { get; }

    public event Action? LoginSucceeded;

    public void Reset()
    {
        Login        = "";
        Password     = "";
        ErrorMessage = "";
        HasError     = false;
        IsLoading    = false;
    }

    public LoginViewModel(PCConfiguratorContext db, UserSession session)
    {
        _db = db;
        _session = session;
        LoginCommand = new AsyncRelayCommand(LoginAsync);
    }

    private async Task LoginAsync()
    {
        ErrorMessage = "";
        HasError = false;
        IsLoading = true;

        try
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Login == Login && u.Password == Password);

            if (user == null)
            {
                ErrorMessage = "Неверный логин или пароль";
                HasError = true;
                return;
            }

            _session.UserId = user.UserId;
            _session.Login = user.Login;
            _session.Role = user.Role;

            LoginSucceeded?.Invoke();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
