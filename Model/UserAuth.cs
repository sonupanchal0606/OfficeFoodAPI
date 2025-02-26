using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OfficeFoodAPI.Model
{
    public class UserAuth
    {

        [Key]
        public Guid userauthid { get; set; } = Guid.NewGuid();

        [ForeignKey("User")]
        public Guid userid { get; set; }
        [JsonIgnore] // Prevents serialization of this navigation property
        public User user { get; set; }

        public string passwordHash { get; set; } // Securely store hashed password
        public string? refreshToken { get; set; } // Optional for JWT refresh tokens
        public DateTime? refreshTokenExpiry { get; set; }
        
    }
}
