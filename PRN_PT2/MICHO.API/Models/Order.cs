using System;
using System.Collections.Generic;

namespace MICHO.API.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? Status { get; set; }

    public DateTime? OrderDate { get; set; }

    public int? CustomerId { get; set; }

    public int? EmpId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Emp { get; set; }

    public virtual ICollection<OrderDetailIceCream> OrderDetailIceCreams { get; set; } = new List<OrderDetailIceCream>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
