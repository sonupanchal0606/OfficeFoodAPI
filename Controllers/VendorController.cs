using Microsoft.AspNetCore.Mvc;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Handlers;
using OfficeFoodAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly VendorHandler _context;

        public VendorController(FoodDbContext context)
        {
            _context = new VendorHandler(context);
        }

        /// <summary>
        /// get list of all vendors with their details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVendors()
        {
            try
            {
                var data = await _context.GetVendors();
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

        // locations served by vendor
        // menu items of vendor
        // GetVendorCompleteDetails
        /// <summary>
        ///  Get Vendor Complete Details by vendor id
        /// </summary>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        [HttpGet("{vendorid}")]
        public async Task<IActionResult> GetVendorById(Guid vendorid)
        {
            try
            {
                var data = await _context.GetVendorById(vendorid);
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
        /// Add vendor to the data base without menu item
        /// </summary>
        /// <param companyname="value"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostVendor([FromBody] Vendor_post value)
        {
            try
            {
                var data = await _context.PostVendor(value);
                if (data != null)
                    return Created();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // add location to the serving location
        // add menu items (----> add price)
        /// <summary>
        /// Update location served by the vendor
        /// </summary>
        /// <param companyname="vendorid"></param>
        /// <param companyname="value"></param>
        /// <returns></returns>
        [HttpPut("{vendorid}")]
        public async Task<IActionResult> PutVendor(Guid vendorid, [FromBody] Vendor_post value)
        {
            try
            {
                var data = await _context.PutVendor(vendorid, value);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        /// <summary>
        /// delete vendor from the DB
        /// </summary>
        /// <param companyname="vendorid"></param>
        /// <returns></returns>
        [HttpDelete("{vendorid}")]
        public async Task<IActionResult> DeleteVendor(Guid vendorid)
        {
            try
            {
                var data = await _context.DeleteVendor(vendorid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


       
        /// <summary>
        /// list of companies served by vendorid
        /// </summary>
        /// <param companyname="vendorid"></param>
        /// <returns></returns>
        [HttpGet("CompaniesServedByVendor/{vendorid}")]
        public async Task<IActionResult> GetCompaniesServedByVendor(Guid vendorid)
        {
            try
            {
                var data = await _context.GetCompaniesServedByVendor(vendorid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet("MenuItemsServedByVendor/{vendorid}")]
        public async Task<IActionResult> MenuItemsServedByVendor(Guid vendorid)
        {
            try
            {
                var data = await _context.MenuItemsServedByVendor(vendorid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        // see monthly report (company wise) --> (companyid, vendorid, month, year)
        /// <summary>
        /// see monthly report (company wise)
        /// </summary>
        /// <param companyname="vendorid"></param>
        /// <param companyname="compnayid"></param>
        /// <param companyname="month"></param>
        /// <param companyname="year"></param>
        /// <returns></returns>
        [HttpGet("GetMonthlyReport/{vendorid}/{compnayid}/{month}/{year}")]
        public async Task<IActionResult> GetMonthlyReport(Guid vendorid, Guid compnayid, int month, int year)
        {
            try
            {
                var data = await _context.GetMonthlyReport(vendorid, compnayid, month, year);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // all 30 days count with total
        /// <summary>
        /// returns the monthly report of all companies for a given vendor
        /// </summary>
        /// <param companyname="vendorId"></param>
        /// <param companyname="year"></param>
        /// <param companyname="month"></param>
        /// <returns></returns>
        [HttpGet("GetMonthlyReportOfCompanies/{vendorId}")]
        public async Task<IActionResult> GetMonthlyReportOfCompanies(Guid vendorId, int year, int month)
        {
            var report = await _context.GetMonthlyReportOfCompanies(vendorId, year, month);
            if (report == null || !report.Companies.Any())
                return NotFound("No data found for the given vendor and month.");

            return Ok(report);
        }


        // see date wise report (companyid, day, vendorid, month, year)
        /// <summary>
        /// returns a date-wise report for a given company, vendor, month, and year.
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="day"></param>
        /// <param name="vendorId"></param>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        /*        
        {
            "CompanyId": "d290f1ee-6c54-4b01-90e6-d701748f0851",
            "VendorId": "f28ad10a-2c38-4c6b-b51a-31e99a98a174",
            "Day": 15,
            "Month": 2,
            "Year": 2025,
            "TotalPlates": 120,
            "TotalRevenue": 6000.0,
            "ItemWiseCounts": {
                "VegThali": 50,
                "NonVegThali": 30,
                "VegSpecial": 20,
                "NonVegSpecial": 20
            }
        }
        */
        [HttpGet("GetDateWiseReport/{vendorId}")]
        public async Task<IActionResult> GetDateWiseReport(Guid companyId, int day, Guid vendorId, int month, int year)
        {
            var report = await _context.GetDateWiseReport(companyId, day, vendorId, month, year);
            if (report == null)
                return NotFound("No data found for the given criteria.");

            return Ok(report);
        }


        // --------- to do later ---------
        // update order status (received, prepared, delivered)
        // update payment status (pending, prepaid, postpaid)
        // see order status
        // see payment status

    }
}
