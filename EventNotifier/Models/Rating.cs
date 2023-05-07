using System.ComponentModel.DataAnnotations;

namespace EventNotifier.Models
{
    public class Rating
    {
        public int Id { get; set; }
        [Required]
        public virtual User User { get; set; }
        [Required]
        public byte RatingNumber { get; set; }
        [Required]
        public virtual Event Event {get; set;}
    }
}
