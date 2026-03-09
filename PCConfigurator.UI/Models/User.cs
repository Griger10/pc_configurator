using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public class User
{
    public int UserId { get; set; }

    /// <summary>Уникальный логин пользователя</summary>
    public string Login { get; set; } = null!;

    /// <summary>Хэш пароля (BCrypt)</summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>Роль: "user" или "admin"</summary>
    public string Role { get; set; } = "user";

    public virtual ICollection<Configuration> Configurations { get; set; } = new List<Configuration>();
}
