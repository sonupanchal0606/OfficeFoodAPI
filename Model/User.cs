using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;


namespace OfficeFoodAPI.Model
{
    public class User
    {
        [Key]
        public Guid userid { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }

        public Guid companyid { get; set; } // if type = employee then store companyid of that employee
        [JsonIgnore] // Prevents serialization of this navigation property
        public Company Company { get; set; }

        // Store Coordinates as Geometry (Point)
        public Point? coordinate { get; set; } // defined SRID 4326 in model builder

        [ForeignKey("UserType")]
        public Guid usertypeid { get; set; }
        public UserType? usertype { get; set; } // Vendor, Employee, Company, Admin
        public DateTime createdat { get; set; }
        public DateTime upatedat { get; set; }
    }



    public class User_Employee_post
    {
        public string? name { get; set; }
        public string? email { get; set; }
    }

    public class RegisterRequest
    {
        public string name { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public Guid companyid { get; set; }
        public Guid usertypeid { get; set; }
        public string password { get; set; }
    }

    public class LoginRequest
    {
        public string email { get; set; }
        public string password { get; set; }
    }

    // When the access token expires, the user will send the refresh token to get a new access token.
    public class RefreshTokenRequest
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

    public class LogoutRequest
    {
        public string RefreshToken { get; set; } // The refresh token to revoke
    }

}
