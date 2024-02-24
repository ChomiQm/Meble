using System.ComponentModel.DataAnnotations;

namespace Meble.Server.Models;

public partial class OrderFurniture
{
    [Key]
    public int OrderFurnitureId { get; set; }
    public int? OrderId { get; set; }
    public virtual Furniture Furniture { get; set; }
    public int Quantity { get; set; }
    public virtual ClientOrder? Order { get; set; }
}
