using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;

namespace EventNotifier.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
