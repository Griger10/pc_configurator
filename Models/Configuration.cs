using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class Configuration
{
    public int ConfigurationId { get; set; }

    public string Name { get; set; } = null!;

    public int ProcessorId { get; set; }

    public int MotherboardId { get; set; }

    public int? Gpuid { get; set; }

    public DateTime CreatedDate { get; set; }

    public decimal TotalPrice { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<ConfigurationRam> ConfigurationRams { get; set; } = new List<ConfigurationRam>();

    public virtual ICollection<ConfigurationStorage> ConfigurationStorages { get; set; } = new List<ConfigurationStorage>();

    public virtual Gpu? Gpu { get; set; }

    public virtual Motherboard Motherboard { get; set; } = null!;

    public virtual Processor Processor { get; set; } = null!;
}
