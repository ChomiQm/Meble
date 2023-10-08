using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class FurnitureOpinion
{
    public int FurnitureOpinionId { get; set; }

    public int FurnitureOpinionFurnitureId { get; set; }

    public int FurnitureOpinionRating { get; set; }

    public int? FurnitureOpinionUserPrimaryId { get; set; }

    public string? FurnitureOpinionComment { get; set; }

    public DateTime? FurnitureOpinionDateOfUpdate { get; set; }

    public virtual Furniture FurnitureOpinionFurniture { get; set; } = null!;

    public virtual User? FurnitureOpinionUserPrimary { get; set; }
}
