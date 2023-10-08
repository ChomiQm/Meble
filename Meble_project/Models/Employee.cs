using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public DateTime EmployeeDateOfEnployment { get; set; }

    public string EmployeePosition { get; set; } = null!;

    public int EmployeeStoreId { get; set; }

    public int EmployeePersonId { get; set; }

    public virtual Person EmployeePerson { get; set; } = null!;

    public virtual Store EmployeeStore { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; } = new List<Task>();
}
