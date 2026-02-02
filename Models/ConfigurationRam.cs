using System;
using System.Collections.Generic;

namespace Курсовой_Конфигуратор_ПК.Models;

public partial class ConfigurationRam
{
    public int ConfigurationRamid { get; set; }

    public int ConfigurationId { get; set; }

    public int Ramid { get; set; }

    public int Quantity { get; set; }

    public virtual Configuration Configuration { get; set; } = null!;

    public virtual Ram Ram { get; set; } = null!;
}
