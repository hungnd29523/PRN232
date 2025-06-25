using System;
using System.Collections.Generic;

namespace MICHO.API.Entity;

public partial class OrderDetailIceCream
{
    public int OrderDetailId { get; set; }

    public int? OrderId { get; set; }

    public int IceId { get; set; }

    public virtual IceCream Ice { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;
}
