using Microsoft.EntityFrameworkCore;
using OfficeFoodAPI.Model;
using NetTopologySuite;
using NetTopologySuite.Geometries;


namespace OfficeFoodAPI.Data
{
    public class FoodDbContext : DbContext
    {
        public FoodDbContext(DbContextOptions<FoodDbContext> options) : base(options) { }
        public DbSet<EmployeeOrderHistory> employee_history_mstr { get; set; }
        public DbSet<User> user_mstr { get; set; }
        // public DbSet<Payment> payment_mstr { get; set; }
        public DbSet<Vendor> vendor_mstr { get; set; }
        public DbSet<Company> company_mstr { get; set; }
        public DbSet<MenuItem> menuitem_mstr { get; set; }
        public DbSet<UserType> usertype_mstr { get; set; }
        public DbSet<UserAuth> userauth_mstr { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(l => l.coordinate)
                .HasColumnType("geometry(Point, 4326)"); // Specify spatial type

            base.OnModelCreating(modelBuilder);
        }

    }
}
