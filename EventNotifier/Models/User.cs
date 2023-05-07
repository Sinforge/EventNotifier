using System.ComponentModel.DataAnnotations;

namespace EventNotifier.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        
        [Required] 
        public string Password { get; set; } = null!;

        public Role Role { get; set; } = Role.DefaultUser;

        public string? ConfirmCode { get; set; } = null!;
        public bool EmailConfirmed { get; set; } = false;

        public ICollection<Event> EventSubscriptions { get; set; } = new List<Event> ();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();



    }
}
