using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meble.Server.Models;

public partial class FurnitureCategory
{
    [Key]
    public int CategoryId { get; set; }
    [Required]
    public string CategoryName { get; set; } = null!;
    public int CategoryFurnitureId { get; set; }
    public DateTime? CategoryDateOfUpdate { get; set; }
    [JsonIgnore]
    public virtual Furniture? CategoryFurniture { get; set; }
}
