using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class StoreLevel
{
    public int StoreLevelId { get; set; }

    public int StoreLevelFurnitureId { get; set; }

    public int StoreLevelStoreId { get; set; }

    public int? StoreLevel1 { get; set; }

    public DateTime? StoreLevelDateOfUpdate { get; set; }

    public virtual Furniture StoreLevelFurniture { get; set; } = null!;

    public virtual Store StoreLevelStore { get; set; } = null!;
}
