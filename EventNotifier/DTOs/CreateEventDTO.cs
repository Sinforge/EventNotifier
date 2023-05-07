using EventNotifier.Models;

namespace EventNotifier.DTOs
{
    public class CreateEventDTO
    {
        public string Name { get; set; } = null!;
        public DateTime Date { get; set; }

        public Point Point { get; set; }
        public string Category { get; set; }

        public string Description { get; set; } = null!;

        public long? MaxSubscribers { get; set; }


    }
    public record Point(float x, float y);
}
