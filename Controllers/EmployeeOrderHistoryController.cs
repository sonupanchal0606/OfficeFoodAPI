using Microsoft.AspNetCore.Mvc;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Handlers;
using OfficeFoodAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeOrderHistoryController : ControllerBase
    {
        private readonly EmployeeOrderHistoryHandler _context;

        public EmployeeOrderHistoryController(FoodDbContext context)
        {
            _context = new EmployeeOrderHistoryHandler(context);
        }

        /*        // PutEmployee
                [HttpPut("PutEmployee")]
                public async Task<IActionResult> PutEmployee([FromBody] EmployeeUpdateRequest request)
                {
                    var result = await _context.UpdateEmployee(request);
                    if (!result) return BadRequest("Employee update failed.");

                    return Ok("Employee updated successfully.");
                }*/

        // PatchEmployee

        /*        [HttpPatch("PatchEmployee/{employeeId}")]
                public async Task<IActionResult> PatchEmployee(Guid employeeId, [FromBody] JsonPatchDocument<Employee> patchDocument)
                {
                    var result = await _context.PatchEmployee(employeeId, patchDocument);
                    if (!result) return NotFound("Employee not found or update failed.");

                    return Ok("Employee updated successfully.");
                }*/

        // ChangeItemStatus

        // subscribe for a plate from current date to date y or whole month
        [HttpPost("SubscribePlate")]
        public async Task<IActionResult> SubscribePlate([FromBody] PlateSubscriptionRequest request)
        {
            var result = await _context.SubscribePlate(request);
            if (result != null) return BadRequest("Subscription failed.");

            return Ok("Subscription successful.");
        }

        // today I ll take lucnh ---> place order 
        [HttpPost("PlaceOrderToday")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            try
            {
                var result = await _context.PlaceOrderAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        // today I wont take lunch ---> cancel order
        [HttpPost("CancelOrderToday")]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
        {
            try
            {
                var result = await _context.CancelOrderAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        // get todays status (day x, month y, year z)

        // GetMonthlyDetails (month x, year y, userid)
        [HttpGet("GetMonthlyReport")]
        public async Task<IActionResult> GetMonthlyReport([FromQuery] int month, [FromQuery] int year, [FromQuery] Guid userId)
        {
            try
            {
                var report = await _context.GenerateMonthlyReportAsync(month, year, userId);
                return Ok(report);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("DownloadMonthlyReport/pdf")]
        public async Task<IActionResult> DownloadMonthlyReport([FromQuery] int month, [FromQuery] int year, [FromQuery] Guid userId)
        {
            try
            {
                var pdfFile = await _context.GenerateMonthlyReportPdfAsync(month, year, userId);
                return File(pdfFile, "application/pdf", $"Monthly_Report_{year}_{month}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }



    }
}
