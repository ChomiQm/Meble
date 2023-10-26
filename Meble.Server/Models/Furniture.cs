

using System.ComponentModel.DataAnnotations;

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

    public virtual ICollection<FurnitureCategory> FurnitureCategories { get; set; } = new List<FurnitureCategory>();

    public virtual ICollection<FurniturePhoto> FurniturePhotos { get; set; } = new List<FurniturePhoto>();

    public virtual ICollection<OrderFurniture> OrderFurnitures { get; set; } = new List<OrderFurniture>();
}
