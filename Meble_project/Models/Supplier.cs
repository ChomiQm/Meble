using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public int SupplierPersonId { get; set; }

    public string SupplierCompanyName { get; set; } = null!;

    public string SupplierCompanyAddress { get; set; } = null!;

    public string SupplierCompanyPhone { get; set; } = null!;

    public string SupplierCompanyMail { get; set; } = null!;

    public DateTime? SupplierDateOfUpdate { get; set; }

    public virtual ICollection<SupplierOrder> SupplierOrders { get; } = new List<SupplierOrder>();

    public virtual Person SupplierPerson { get; set; } = null!;
}
