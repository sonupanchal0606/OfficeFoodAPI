using Microsoft.AspNetCore.Mvc;
using OfficeFoodAPI.Data;
using OfficeFoodAPI.Handlers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OfficeFoodAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        private readonly PaymentHandler _context;

        public PaymentController(FoodDbContext context)
        {
            _context = new PaymentHandler(context);
        }
    }
}
