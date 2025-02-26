using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeFoodAPI.Handlers;
using OfficeFoodAPI.Model;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserAuthHandler _authService;

        public UserAuthController(UserAuthHandler authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        /// <summary>
        /// TO register an user
        /// </summary>
        /// <param companyname="request"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = new User
            {
                userid = Guid.NewGuid(),
                name = request.name,
                email = request.email,
                address = request.address,
                companyid = request.companyid,
                usertypeid = request.usertypeid,
                createdat = DateTime.UtcNow,
                upatedat = DateTime.UtcNow
            };

            bool success = await _authService.Register(user, request.password);
            if (!success)
                return BadRequest("Email already exists");

            return Ok("User registered successfully");
        }


        [AllowAnonymous]
        /// <summary>
        /// to login
        /// </summary>
        /// <param companyname="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.Login(request.email, request.password);
            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { Token = token });
        }

        /// <summary>
        /// When the access token expires, the user will send the refresh token to get a new access token.
        /// </summary>
        /// <param companyname="request"></param>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var data = await RefreshToken(request);

            return Ok(data);
        }

        /// <summary>
        /// Logout & Invalidate Refresh Token
        /// </summary>
        /// <param companyname="request"></param>
        /// <returns></returns>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var data = await Logout(request);
            return Ok(data);
        }

    }
}
