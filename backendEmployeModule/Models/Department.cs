using System;
using System.Collections.Generic;

namespace backendEmployeModule.Models;

public partial class Department
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Employe> Employes { get; } = new List<Employe>();
}
