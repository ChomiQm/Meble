using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class FurnitureCategory
{
    public int FurnitureCategoryId { get; set; }

    public string FurnitureCategoryName { get; set; } = null!;

    public int FurnitureCategoryFurnitureId { get; set; }

    public DateTime? FurnitureCategoryDateOfUpdate { get; set; }

    public virtual Furniture FurnitureCategoryFurniture { get; set; } = null!;
}
