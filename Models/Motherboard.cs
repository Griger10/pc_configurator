using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class Motherboard
{
    public int MotherboardId { get; set; }

    public int ManufacturerId { get; set; }

    public string Model { get; set; } = null!;

    public string Socket { get; set; } = null!;

    public string Chipset { get; set; } = null!;

    public string Ramtype { get; set; } = null!;

    public int MaxRam { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<Configuration> Configurations { get; set; } = new List<Configuration>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
