using System;
using System.Collections.Generic;

namespace MICHO.API.Entity;

public partial class Employee
{
    public int EmpId { get; set; }

    public string? EmpName { get; set; }

    public string? Idcard { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
