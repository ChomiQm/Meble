using System.ComponentModel.DataAnnotations;

namespace Meble.Server.Models;

public partial class ClientOrder
{
    [Key]
    public int OrderId { get; set; }

    public required string OrderUserId { get; set; }

    public DateOnly? OrderDateOfOrder { get; set; }

    public DateTime? OrderDateOfUpdate { get; set; }

    public virtual ICollection<OrderFurniture> OrderFurnitures { get; set; } = new List<OrderFurniture>();
    [Required]
    public virtual User OrderUser { get; set; } = null!;
}
