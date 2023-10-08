using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Manufacturer
{
    public int ManufacturerId { get; set; }

    public string ManufacturerCompanyName { get; set; } = null!;

    public string ManufacturerAddress { get; set; } = null!;

    public string ManufacturerPhone { get; set; } = null!;

    public string ManufacturerMail { get; set; } = null!;

    public DateTime? ManufactureDateOfUpdate { get; set; }

    public virtual ICollection<Furniture> Furnitures { get; } = new List<Furniture>();
}
