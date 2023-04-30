using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace EventNotifier.Models
{
    public class Event
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; } = null!;

        public long CurrentSubscribers { get; set; }

        public long? MaxSubscribers { get; set; }

        public ICollection<User> Subscribers { get; set; } = new List<User>();



    }
}
