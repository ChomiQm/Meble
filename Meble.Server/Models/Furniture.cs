using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meble.Server.Models;

public partial class Furniture
{
    [Key]
    public int FurnitureId { get; set; }
    [Required]
    public string FurnitureName { get; set; } = null!;
    public decimal FurniturePrice { get; set; }
    public DateOnly? FurnitureDateOfAddition { get; set; }
    public string? FurnitureDescription { get; set; }
    public DateTime? FurnitureDateOfUpdate { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int Quantity { get; set; }
    [JsonIgnore]
    public virtual ICollection<FurnitureCategory> FurnitureCategories { get; set; } = new List<FurnitureCategory>();
    [JsonIgnore]
    public virtual ICollection<FurniturePhoto> FurniturePhotos { get; set; } = new List<FurniturePhoto>();
    [JsonIgnore]
    public virtual ICollection<OrderFurniture> OrderFurnitures { get; set; } = new List<OrderFurniture>();
}
