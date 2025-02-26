using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Handlers;
using OfficeFoodAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeController : ControllerBase
    {
        private readonly UserTypeHandler _context;

        public UserTypeController(FoodDbContext context)
        {
            _context = new UserTypeHandler(context);
        }

        [AllowAnonymous] 
        [HttpGet]
        public async Task<IActionResult> GetUserTypes()
        {
            try
            {
                var data = await _context.GetUserTypes();
                return Ok(data);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No"))
                    return NotFound();
                else
                    return BadRequest(ex);
            }
        }

        [HttpGet("{UserTypeId}")]
        public async Task<IActionResult> GetUserTypeById(Guid UserTypeId)
        {
            try
            {
                var data = await _context.GetUserTypeById(UserTypeId);
                return Ok(data);

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("No"))
                    return NotFound();
                else
                    return BadRequest(ex);
            }
        }


        [HttpPost]
        public async Task<IActionResult> PostUserType([FromBody] UserType_post value)
        {
            try
            {
                var data = await _context.PostUserType(value);
                if (data != null)
                    return Created();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{UserTypeId}")]
        public async Task<IActionResult> PutUserType(Guid UserTypeId, [FromBody] UserType_post value)
        {
            try
            {
                var data = await _context.PutUserType(UserTypeId, value);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{UserTypeId}")]
        public async Task<IActionResult> DeleteUserType(Guid UserTypeId)
        {
            try
            {
                var data = await _context.DeleteUserType(UserTypeId);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
