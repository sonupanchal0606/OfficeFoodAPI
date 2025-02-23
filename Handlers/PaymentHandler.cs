using OfficeFoodAPI.Data;

namespace OfficeFoodAPI.Handlers
{
    public class PaymentHandler
    {
        private FoodDbContext _context;

        public PaymentHandler(FoodDbContext context)
        {
            _context = context;
        }
    }
}
