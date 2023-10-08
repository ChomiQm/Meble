using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class ClientOrder
{
    public int ClientOrderId { get; set; }

    public int ClientOrderPersonId { get; set; }

    public int ClientOrderFurnitureId { get; set; }

    public DateTime? ClientOrderDateOfOrder { get; set; }

    public int ClientOrderQuantity { get; set; }

    public decimal ClientOrderUnitPrice { get; set; }

    public DateTime? ClientOrderDateOfUpdate { get; set; }

    public virtual Furniture ClientOrderFurniture { get; set; } = null!;

    public virtual Person ClientOrderPerson { get; set; } = null!;
}
