using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class Ram
{
    public int Ramid { get; set; }

    public int ManufacturerId { get; set; }

    public string Model { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int Capacity { get; set; }

    public int Frequency { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<ConfigurationRam> ConfigurationRams { get; set; } = new List<ConfigurationRam>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
