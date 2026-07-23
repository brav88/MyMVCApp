using Microsoft.EntityFrameworkCore;
using MyWebApp.Models;


namespace MyWebApp.DatabaseHelper
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<CreditCard> CreditCard { get; set; }
    }
}
