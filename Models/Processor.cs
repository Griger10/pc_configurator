using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class Processor
{
    public int ProcessorId { get; set; }

    public int ManufacturerId { get; set; }

    public string Model { get; set; } = null!;

    public string Socket { get; set; } = null!;

    public int Cores { get; set; }

    public decimal Frequency { get; set; }

    public virtual ICollection<Configuration> Configurations { get; set; } = new List<Configuration>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
