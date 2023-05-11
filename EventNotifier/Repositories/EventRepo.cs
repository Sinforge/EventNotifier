using EventNotifier.Data;
using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

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

        public void ChangeToComplete(int @eventId)
        {
            var @event = _context.Events.FirstOrDefault(e => e.Id == @eventId);
            @event.isCompleted = true;
            _context.SaveChanges();
        }

        public void ClearNotifications()
        {
            foreach(var not in _context.Notifications)
            {
                if (not.IsChecked)
                {
                    _context.Notifications.Remove(not);
                }
             

            }
        }

        public bool CreateEvent(Event @event)
        {
            _context.Events.Add(@event);
            _logger.LogInformation("New event added");
            return _context.SaveChanges() >= 0;
        }

        public void CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);
        }

        public Event? GetEventById(int id)
        {
            return _context.Events.FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Event> GetEvents()
        {
            return _context.Events.ToList();
        }

        public IEnumerable<Event> GetEventsByCoords(Coordinate coord, double distance)
        {
            var point = new Point(coord);
            var events = _context.Events.ToList();
            return from @event in events where @event.Point.Distance(point) <= distance select @event;
        }



        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }

    }
}
