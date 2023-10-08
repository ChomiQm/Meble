using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Furniture
{
    public int FurnitureId { get; set; }

    public string FurnitureName { get; set; } = null!;

    public decimal FurniturePrice { get; set; }

    public DateTime? FurnitureDateOfAddition { get; set; }

    public string? FurnitureDescription { get; set; }

    public int FurnitureManufacturerId { get; set; }

    public int FurnitureStoreId { get; set; }

    public DateTime? FurnitureDateOfUpdate { get; set; }

    public virtual ICollection<ClientOrder> ClientOrders { get; } = new List<ClientOrder>();

    public virtual ICollection<FurnitureCategory> FurnitureCategories { get; } = new List<FurnitureCategory>();

    public virtual Manufacturer FurnitureManufacturer { get; set; } = null!;

    public virtual ICollection<FurnitureOpinion> FurnitureOpinions { get; } = new List<FurnitureOpinion>();

    public virtual Store FurnitureStore { get; set; } = null!;

    public virtual ICollection<Photo> Photos { get; } = new List<Photo>();

    public virtual ICollection<StoreLevel> StoreLevels { get; } = new List<StoreLevel>();

    public virtual ICollection<SupplierOrder> SupplierOrders { get; } = new List<SupplierOrder>();
}
