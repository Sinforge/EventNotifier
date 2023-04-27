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

        public Role role { get; set; } = Role.DefaultUser;
        public bool EmailConfirmed { get; set; } = false;


    }
}
