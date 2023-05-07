using System.ComponentModel.DataAnnotations;

namespace EventNotifier.Models
{
    public class Rating
    {
        public int Id { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public byte RatingNumber { get; set; }
        [Required]
        public Event Event {get; set;}
    }
}
