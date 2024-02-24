using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Meble.Server.Models
{
    public class User : IdentityUser
    {
        public DateTime? UserDateOfUpdate { get; set; }
        public virtual UserData? UserDatas { get; set; }
        [JsonIgnore]
        public virtual ICollection<ClientOrder> ClientOrders { get; set; } = new List<ClientOrder>();
    }
}