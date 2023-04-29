using EventNotifier.Data;
using EventNotifier.Models;
using Microsoft.Extensions.Logging;

namespace EventNotifier.Repositories
{
    public class EventRepo : IEventRepo
    {
        private readonly ILogger<EventRepo> _logger;
        private readonly ApplicationDbContext _context;
        public EventRepo(ILogger<EventRepo> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public bool CreateEvent(Event @event)
        {
            _context.Events.Add(@event);
            _logger.LogInformation("New event added");
            return _context.SaveChanges() >= 0;
        }

        public Event? GetEventById(int id)
        {
            return _context.Events.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Event> GetEvents()
        {
            return _context.Events.ToList();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
    }
}
