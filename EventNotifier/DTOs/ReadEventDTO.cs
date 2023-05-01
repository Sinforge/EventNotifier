using EventNotifier.Models;
using System.ComponentModel.DataAnnotations;

namespace EventNotifier.DTOs
{
    public class ReadEventDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime Date { get; set; }
        public string Description { get; set; } = null!;

        public long? MaxSubscribers { get; set; }

        public long CurrentSubscribers { get; set; }

    }
}
