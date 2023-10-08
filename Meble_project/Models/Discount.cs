using System;
using System.Collections.Generic;

namespace inzynierka_geska.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string DiscountName { get; set; } = null!;

    public decimal DiscountPercentageValue { get; set; }

    public DateTime DiscountStartDate { get; set; }

    public DateTime DiscountEndDate { get; set; }

    public int DiscountUserPrimaryId { get; set; }

    public DateTime? DiscountDateOfUpdate { get; set; }

    public virtual User DiscountUserPrimary { get; set; } = null!;
}
