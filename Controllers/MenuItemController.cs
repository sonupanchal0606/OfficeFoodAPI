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

        
        /// <summary>
        /// Get all menu items. Admin can access all the menu options available
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Admin can get the details of menu item based on menuid
        /// </summary>
        /// <param name="menu_itemid"></param>
        /// <returns></returns>
        [HttpGet("{menu_itemid}")]
        public async Task<IActionResult> GetMenuById(Guid menu_itemid)
        {
            try
            {
                var data = await _context.GetMenuById(menu_itemid);
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





        /// <summary>
        /// vendor can access his/her menu items based on menuid
        /// Admin/Vendor/Company can get the details of menu item based on menuid
        /// </summary>
        /// <param name="vendorid"></param>
        /// <param name="menu_itemid"></param>
        /// <returns></returns>
        [HttpGet("{vendorid}/{menu_itemid}")]
        public async Task<IActionResult> GetMenuByVendorId(Guid vendorid, Guid menu_itemid)
        {
            try
            {
                var data = await _context.GetMenuByVendorId(vendorid, menu_itemid);
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

        
        /// <summary>
        /// only vendor can add item to menu list
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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

        // only vendor can update the item
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

        // Only vendor can delete the item
        [HttpDelete("{vendorid}/{menuitemid}")]
        public async Task<IActionResult> DeleteMenuItem(Guid vendorid, Guid menuitemid)
        {
            try
            {
                var data = await _context.DeleteMenuItem(vendorid, menuitemid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
