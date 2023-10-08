using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Store
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public string StoreAddress { get; set; } = null!;

    public string StorePhone { get; set; } = null!;

    public DateTime? StoreDateOfUpdate { get; set; }

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();

    public virtual ICollection<Furniture> Furnitures { get; } = new List<Furniture>();

    public virtual ICollection<StoreLevel> StoreLevels { get; } = new List<StoreLevel>();
}
