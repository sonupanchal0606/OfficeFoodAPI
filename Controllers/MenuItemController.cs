using Microsoft.AspNetCore.Mvc;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Handlers;
using OfficeFoodAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeFoodAPI.Controllers
{
    //[Route("api/[Action]")]
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly MenuItemHandler _context;

        public MenuItemController(FoodDbContext context)
        {
            _context = new MenuItemHandler(context);
        }

        [HttpGet("GetMenuItems")]
        public async Task<IActionResult> GetMenuItems()
        {
            try
            {
                var data = await _context.GetMenuItem();
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

        [HttpGet("{menuid}")]
        public async Task<IActionResult> GetMenuById(Guid menuid)
        {
            try
            {
                var data = await _context.GetMenuById(menuid);
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
        public async Task<IActionResult> PostMenuItem([FromBody] MenuItem_post value)
        {
            try
            {
                var data = await _context.PostMenuItem(value);
                if (data != null)
                    return Created();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{menuitemid}")]
        public async Task<IActionResult> PutMenuItem(Guid menuitemid, [FromBody] MenuItem_post value)
        {
            try
            {
                var data = await _context.PutMenuItem(menuitemid, value);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{menuitemid}")]
        public async Task<IActionResult> DeleteMenuItem(Guid menuitemid)
        {
            try
            {
                var data = await _context.DeleteMenuItem(menuitemid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
