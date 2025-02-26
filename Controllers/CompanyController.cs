using Microsoft.AspNetCore.Mvc;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Handlers;
using OfficeFoodAPI.HelperClasses;
using OfficeFoodAPI.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeFoodAPI.Controllers
{
    //[Route("api/[Action]")]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyHandler _context;

        public CompanyController(FoodDbContext context)
        {
            _context = new CompanyHandler(context);
        }

        [HttpGet("GetAllCompnsies")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.GetAll();
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

        [HttpGet("GetCompanyById/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var data = await _context.Get(id);
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

      
        [HttpPost("PostCompany")]
        public async Task<IActionResult> Post([FromBody] Company_post value)
        {
            try
            {
                var data = await _context.Post(value);
                if (data != null)
                return Created();
                else return BadRequest();
            }
            catch (Exception ex)
            {
                    return BadRequest(ex);
            }
        }

        [HttpPut("UpdateCompany/{companyid}")]
        public async Task<IActionResult> Put(Guid companyid, [FromBody] Company_post value)
        {
            try
            {
                var data = await _context.Put(companyid, value);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        /// <summary>
        /// Get Company Vendor Details
        /// </summary>
        /// <param companyname="companyid"></param>
        /// <returns></returns>
        [HttpGet("GetCompanyVendorDetails/{companyid}")]
        public async Task<IActionResult> GetCompanyVendorDetails(Guid companyid)
        {
            try
            {
                var data = await _context.GetCompanyVendorDetails(companyid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /*HR related queries*/
        /// <summary>
        /// assign vendor to the company from current date ------> (current date pending) --> need discussion
        /// subscribe to a particular menu, particular vendor for month for all employees whoever is in the list from current date
        /// </summary>
        /// <param companyname="companyid"></param>
        /// <param companyname="vendorid"></param>
        /// <returns></returns>
        [HttpPatch("AssignVendorToCompany/{companyid}/{vendorid}")]
        public async Task<IActionResult> AssignVendor(Guid companyid, Guid vendorid)
        {
            try
            {
                var data = await _context.AssignVendor(companyid, vendorid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// remove assigned vendor to the company
        /// </summary>
        /// <param name="companyid"></param>
        /// <returns></returns>
        [HttpPatch("RemoveVendor/{companyid}")]
        public async Task<IActionResult> RemoveVendor(Guid companyid)
        {
            try
            {
                var data = await _context.RemoveVendor(companyid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// update/change the vendor assigned to this company id with new vendorid
        /// </summary>
        /// <param name="companyid"></param>
        /// <param name="vendorid"></param>
        /// <returns></returns>
        [HttpPut("UpdateVendor/{companyid}")]
        public async Task<IActionResult> UpdateVendor(Guid companyid, Guid NewVendorid)
        {
            try
            {
                var data = await _context.UpdateVendor(companyid, NewVendorid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        /// <summary>
        /// add employee to the company
        /// </summary>
        /// <param companyname="companyid"></param>
        /// <param companyname="emailid"></param>
        /// <returns></returns>
        [HttpPatch("AddEmployeeToCompany/{companyid}")]
        public async Task<IActionResult> AddEmployee(Guid companyid, string emailid)
        {
            try
            {
                var data = await _context.AddEmployeeUser(companyid, emailid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// remove employee from company
        /// </summary>
        /// <param companyname="userid"></param>
        /// <returns></returns>
        [HttpDelete("RemoveEmployeeFromCompany/{userid}")]
        public async Task<IActionResult> RemoveEmployee(Guid userid)
        {
            try
            {
                var data = await _context.RemoveEmployeeUser(userid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// get employee details based on emailid
        /// </summary>
        /// <param companyname="emailid"></param>
        /// <returns></returns>
        [HttpGet("GetEmployeeDetails/{emailid}")]
        public async Task<IActionResult> GetEmployeeDetails(string emailid)
        {
            try
            {
                var data = await _context.GetEmployeeDetails(emailid);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        /// <summary>
        /// Get Monthly Report of the Employee (pending)
        /// </summary>
        /// <param companyname="companyid"></param>
        /// <param companyname="month"></param>
        /// <param companyname="year"></param>
        /// <returns></returns>
        [HttpGet("GetMonthlyReportOfAnEmployee/{month}/{year}")]
        public async Task<IActionResult> GetMonthlyReport([FromQuery] Guid companyid, [FromQuery] Guid userid, int month=1, int year=2025)
        {
            try
            {
                var data = await _context.GetMonthlyReportOfAnEmployee(companyid, userid, month, year);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // see menu options of a particular vendor ----> GetVendorById


        // today order deferent lunch (which plate), (if compnay will pay or employee) ---> special plate on a speical ocassion ---> need more discussion

        // update subsidy by the company -----> can use Put query

        // get monthly record for all employee for a particular company ---> download excel
        [HttpGet("GetMonthlyReportOfAllEmployees")]
        public async Task<IActionResult> GetMonthlyEmployeeReport(Guid companyId, int month, int year, bool downloadAsExcel = false)
        {
            var report = await _context.GetMonthlyEmployeeReport(companyId, month, year);

            if (report == null || !report.EmployeeRecords.Any())
                return NotFound("No data found for the given criteria.");

            if (downloadAsExcel)
            {
                var fileBytes = ExcelHelper.GenerateMonthlyEmployeeReport(report);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"Employee_Monthly_Report_{month}_{year}.xlsx");
            }

            return Ok(report);
        }

        [HttpGet("GetDailyCompanyReport")]
        public async Task<IActionResult> GetDailyCompanyReport(Guid companyId, int day, int month, int year)
        {
            var report = await _context.GetDailyCompanyReport(companyId, day, month, year);
            if (report == null)
                return NotFound("No orders found for the given date.");

            return Ok(report);
        }


    }
}
