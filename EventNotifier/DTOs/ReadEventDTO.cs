

using Newtonsoft.Json;

namespace EventNotifier.DTOs
{
    public class ReadEventDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Description { get; set; } = null!;

        public string Category { get; set; } = null!;
        public long? MaxSubscribers { get; set; }

        public float AverageRating { get; set; }
        public NetTopologySuite.Geometries.Point Point { get; set; }
        public long CurrentSubscribers { get; set; }

    }
}
