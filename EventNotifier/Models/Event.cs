using NetTopologySuite.Geometries;
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
        public Point Point { get; set; } = null!;
        [Required]
        public string Category { get; set; } = null!;
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; } = null!;

        public long? MaxSubscribers { get; set; }


        public bool isCompleted { get; set; } = false;
        public ICollection<User> Subscribers { get; set; } = new List<User>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();




    }

}
