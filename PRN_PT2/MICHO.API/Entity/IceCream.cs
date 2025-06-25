using System;
using System.Collections.Generic;

namespace MICHO.API.Entity;

public partial class IceCream
{
    public int IceId { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public string? Flavor { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<OrderDetailIceCream> OrderDetailIceCreams { get; set; } = new List<OrderDetailIceCream>();
}
