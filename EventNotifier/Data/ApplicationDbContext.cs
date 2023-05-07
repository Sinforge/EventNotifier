using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;

namespace EventNotifier.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Notification> Notifications { get; set; }  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // For working with Location Types
            modelBuilder.HasPostgresExtension("postgis");

            //Seed data
            modelBuilder.Entity<User>()
                        .HasData(
                         new User
                         {
                             Id = 1,
                             Email = "vlad.vlasov77@mail.ru",
                             Password = "Aboba12345",
                             ConfirmCode = null,
                             EmailConfirmed = true,
                             Role = Role.Administration,
                             Username = "Vladislav Vlasov"
                         }
                         );
        }

    }
}
