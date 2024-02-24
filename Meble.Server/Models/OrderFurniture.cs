using System.ComponentModel.DataAnnotations;

namespace Meble.Server.Models;

public partial class OrderFurniture
{
    [Key]
    public int OrderFurnitureId { get; set; }
    public int? OrderId { get; set; }
    public int? FurnitureId { get; set; }
    public int? QuantityOrdered { get; set; }
    public virtual Furniture? Furniture { get; set; }
    public virtual ClientOrder? Order { get; set; }
}
