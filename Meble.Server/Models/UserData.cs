using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Meble.Server.Models
{
    public class UserData
    {
        [Key]
        public string? UserDataId { get; set; } 
        [Required]
        public string UserFirstName { get; set; } = null!;
        [Required]
        public string UserSurname { get; set; } = null!;
        [Required]
        public string UserCountry { get; set; } = null!;
        [Required]
        public string UserTown { get; set; } = null!;
        [JsonIgnore]
        public string? UserId { get; set; } 
        public string? UserStreet { get; set; }
        public int? UserHomeNumber { get; set; }
        public string? UserFlatNumber { get; set; }
        [JsonIgnore]
        public virtual User? User { get; set; } = null!;

    }
}
