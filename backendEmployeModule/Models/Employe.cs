using System;
using System.Collections.Generic;

namespace backendEmployeModule.Models;

public partial class Employe
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public decimal? Pay { get; set; }

    public DateTime? ContractDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? IdDept { get; set; }

    public virtual Department? IdDeptNavigation { get; set; }
}
