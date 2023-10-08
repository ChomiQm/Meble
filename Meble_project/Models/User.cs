using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class User
{
    public int UserPrimaryId { get; set; }

    public string UserLogin { get; set; } = null!;

    public string UserPassword { get; set; } = null!;

    public DateTime? UserDateOfUpdate { get; set; }

    public bool? UserIsEmployee { get; set; }

    public bool? UserIsSupplier { get; set; }

    public int UserPersonId { get; set; }

    public virtual ICollection<Discount> Discounts { get; } = new List<Discount>();

    public virtual ICollection<FurnitureOpinion> FurnitureOpinions { get; } = new List<FurnitureOpinion>();

    public virtual Person UserPerson { get; set; } = null!;
}
