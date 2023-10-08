using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class SupplierOrder
{
    public int SupplierOrderId { get; set; }

    public int SupplierOrderSupplierId { get; set; }

    public int SupplierOrderFurnitureId { get; set; }

    public DateTime? SupplierOrderDateOfOrder { get; set; }

    public int SupplierOrderQuantity { get; set; }

    public decimal SupplierOrderUnitPrice { get; set; }

    public DateTime? SupplierOrderDateOfUpdate { get; set; }

    public virtual Furniture SupplierOrderFurniture { get; set; } = null!;

    public virtual Supplier SupplierOrderSupplier { get; set; } = null!;
}
