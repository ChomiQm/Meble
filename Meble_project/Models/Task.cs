using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Task
{
    public int TaskId { get; set; }

    public int TaskEmployeeId { get; set; }

    public string TaskDescription { get; set; } = null!;

    public DateTime TaskAssignmentDate { get; set; }

    public DateTime? TaskDateOfUpdate { get; set; }

    public virtual Employee TaskEmployee { get; set; } = null!;
}
