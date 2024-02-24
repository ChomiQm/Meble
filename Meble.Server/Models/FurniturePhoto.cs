using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meble.Server.Models;

public partial class FurniturePhoto
{
    [Key]
    public int PhotoId { get; set; }
    public int PhotoFurnitureId { get; set; }
    [Required]
    public string PhotoUrl { get; set; } = null!;
    [Required]
    public string PhotoDescription { get; set; } = null!;
    public DateTime? PhotoDateOfUpdate { get; set; }
    [JsonIgnore]
    public virtual Furniture? PhotoFurniture { get; set; }
}
