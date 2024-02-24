using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meble.Server.Models;

public partial class ClientOrder
{
    [Key]
    public int OrderId { get; set; }
    public required string OrderUserId { get; set; } 
    public DateOnly OrderDateOfOrder { get; set; }
    public DateTime? OrderDateOfUpdate { get; set; }
    public decimal TotalOrderValue { get; set; } 
    public int TotalItemsOrdered { get; set; }
    public string OrderStatus { get; set; } = null!;
    public virtual ICollection<OrderFurniture> OrderFurnitures { get; set; } = new List<OrderFurniture>();
    [JsonIgnore]
    public virtual User? OrderUser { get; set; }
   
}
