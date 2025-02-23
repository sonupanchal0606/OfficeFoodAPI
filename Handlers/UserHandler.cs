using OfficeFoodAPI.Data;

namespace OfficeFoodAPI.Handlers
{
    public class UserHandler
    {
        private FoodDbContext _context;

        public UserHandler(FoodDbContext context)
        {
            _context = context;
        }
    }
}
