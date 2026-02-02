using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class Gpu
{
    public int Gpuid { get; set; }

    public int ManufacturerId { get; set; }

    public string Model { get; set; } = null!;

    public int Memory { get; set; }

    public string MemoryType { get; set; } = null!;

    public int PowerConsumption { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Configuration> Configurations { get; set; } = new List<Configuration>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
