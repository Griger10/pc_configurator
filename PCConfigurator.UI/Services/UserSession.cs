namespace PCConfigurator.UI.Services;

/// <summary>
/// Singleton-сервис: хранит данные текущего авторизованного пользователя.
/// Заполняется LoginViewModel после успешного входа.
/// </summary>
public class UserSession
{
    public int UserId { get; set; }
    public string Login { get; set; } = "";
    public string Role { get; set; } = "user";

    /// <summary>Текущий пользователь является администратором</summary>
    public bool IsAdmin => Role == "admin";

    /// <summary>Пользователь вошёл в систему</summary>
    public bool IsAuthenticated => UserId > 0;

    public void Reset()
    {
        UserId = 0;
        Login  = "";
        Role   = "user";
    }
}
