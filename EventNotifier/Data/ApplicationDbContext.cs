using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;

namespace EventNotifier.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated(); 
        }
    }
}
