using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Photo
{
    public int PhotoId { get; set; }

    public int PhotoFurnitureId { get; set; }

    public byte[] PhotoPhoto { get; set; } = null!;

    public string PhotoDescription { get; set; } = null!;

    public DateTime? PhotoDateOfUpdate { get; set; }

    public virtual Furniture PhotoFurniture { get; set; } = null!;
}
