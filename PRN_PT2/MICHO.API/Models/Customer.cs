using System;
using System.Collections.Generic;

namespace MICHO.API.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? Contact { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
