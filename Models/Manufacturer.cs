using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string Name { get; set; } = null!;

    public string? Country { get; set; }

    public string? Website { get; set; }

    public virtual ICollection<Gpu> Gpus { get; set; } = new List<Gpu>();

    public virtual ICollection<Motherboard> Motherboards { get; set; } = new List<Motherboard>();

    public virtual ICollection<Processor> Processors { get; set; } = new List<Processor>();

    public virtual ICollection<Ram> Rams { get; set; } = new List<Ram>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
