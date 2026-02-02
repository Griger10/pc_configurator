using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class ConfigurationStorage
{
    public int ConfigurationStorageId { get; set; }

    public int ConfigurationId { get; set; }

    public int StorageId { get; set; }

    public int Quantity { get; set; }

    public virtual Configuration Configuration { get; set; } = null!;

    public virtual Storage Storage { get; set; } = null!;
}
