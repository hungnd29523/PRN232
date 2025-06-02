using System;
using System.Collections.Generic;

namespace PRN_PT1.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string? Status { get; set; }

    public DateOnly? OrderDate { get; set; }

    public int? EmpId { get; set; }

    public virtual Employee? Emp { get; set; }
}
