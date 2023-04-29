using EventNotifier.Models;

namespace EventNotifier.Repositories
{
    public interface IEventRepo
    {
        public bool CreateEvent(Event @event);
        public IEnumerable<Event> GetEvents();
        public Event? GetEventById(int id);

        public bool SaveChanges();
    }
}
