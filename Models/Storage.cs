using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class Storage
{
    public int StorageId { get; set; }

    public int ManufacturerId { get; set; }

    public string Model { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int Capacity { get; set; }

    public string Interface { get; set; } = null!;

    public virtual ICollection<ConfigurationStorage> ConfigurationStorages { get; set; } = new List<ConfigurationStorage>();

    public virtual Manufacturer Manufacturer { get; set; } = null!;
}
