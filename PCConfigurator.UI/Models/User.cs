using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public class User
{
    public int UserId { get; set; }
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string Role { get; set; } = "user";

    public virtual ICollection<Configuration> Configurations { get; set; } = new List<Configuration>();
}
