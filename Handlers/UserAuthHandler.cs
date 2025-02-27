using Microsoft.IdentityModel.Tokens;
using OfficeFoodAPI.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using OfficeFoodAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace OfficeFoodAPI.Handlers
{
    public class UserAuthHandler
    {
        private readonly FoodDbContext _context;
        private readonly IConfiguration _configuration;

        public UserAuthHandler(FoodDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // User Registration
        public async Task<bool> Register(User user, string password)
        {
            if (_context.user_mstr.Any(u => u.email == user.email))
                return false; // Email already exists

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var userAuth = new UserAuth
            {
                userid = user.userid,
                passwordHash = hashedPassword
            };

            _context.user_mstr.Add(user); // need to fix this 
            _context.userauth_mstr.Add(userAuth);
            await _context.SaveChangesAsync();
            return true;
        }

        // User Login
        public async Task<AuthResponse> Login(string email, string password)
        {
            var user = await _context.user_mstr.Include(u => u.usertype).FirstOrDefaultAsync(u => u.email == email);
            if (user == null)
                return null; // user not registered

            var userAuth = await _context.userauth_mstr.FirstOrDefaultAsync(ua => ua.userid == user.userid);
            if (userAuth == null || !BCrypt.Net.BCrypt.Verify(password, userAuth.passwordHash))
                return null; // Invalid login

            // Generate Access Token
            var accessToken = GenerateJwtToken(user);

            // Generate Refresh Token
            var refreshToken = GenerateRefreshToken();
            userAuth.refreshToken = refreshToken;
            userAuth.refreshTokenExpiry = DateTime.UtcNow.AddDays(7); // Refresh token valid for 7 days

            // Save refresh token to DB
            await _context.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        // Generate JWT Token
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.userid.ToString()),
            new Claim(ClaimTypes.Email, user.email),
            //new Claim("UserType", user.usertype?.usertype.ToString() ?? "Employee")
            new Claim(ClaimTypes.Role, user.usertype?.usertype.ToString() ?? "Employee") // Ensure role claim is included
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                //expires: DateTime.UtcNow.AddMinutes(15), // Access token valid for 15 min
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Helper Function to Generate a Refresh Token
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<AuthResponse> RefreshToken(RefreshTokenRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
            {
                //return Unauthorized();
                throw new UnauthorizedAccessException("Unauthorized");
            }

            var userId = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var user_auth = await _context.userauth_mstr.FindAsync(userId);
            var user = await _context.user_mstr.FindAsync(userId);

            if (user_auth == null || user_auth.refreshToken != request.RefreshToken || user_auth.refreshTokenExpiry < DateTime.UtcNow)
            {
                // return Unauthorized(); // Invalid refresh token
                throw new UnauthorizedAccessException("Unauthorized");
            }

            // Generate a new access token
            var newAccessToken = GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            // Update refresh token in DB
            user_auth.refreshToken = newRefreshToken;
            user_auth.refreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return (new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        // Extract user details from an expired token to verify the refresh token.
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // Ignore expiry because we are checking manually
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtToken = securityToken as JwtSecurityToken;

            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }


        // 
        public async Task<string> Logout(LogoutRequest request)
        {
            var user = await _context.userauth_mstr.FirstOrDefaultAsync(u => u.refreshToken == request.RefreshToken);
            if (user == null)
                throw new UnauthorizedAccessException("Unauthorized");
                //return Unauthorized();

            user.refreshToken = null;
            user.refreshTokenExpiry = null;
            await _context.SaveChangesAsync();

            return ("Logged out successfully");
        }

    }
}
