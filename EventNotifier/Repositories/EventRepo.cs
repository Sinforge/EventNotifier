using EventNotifier.Data;
using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;
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

        public void ChangeToComplete(Event @event)
        {
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
            return _context.Events.Include(e=> e.Ratings).Include(e => e.Subscribers).FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Event> GetEvents()
        {
            return _context.Events.Include(e => e.Ratings).Include(e => e.Subscribers).ToList();
        }

        public bool SaveChanges()
        {
            return _context.SaveChanges() >= 0;
        }
       /* public List<Rating> GetRatingsBySameEvent(User user)
        {
            return _context.Ratings.Include(r => r.User).Include(r => r.Event)
                .Select(r => r)
                .Where(r => r.User != user && user.Ratings.FirstOrDefault(rating => rating.Event.Id == r.Event.Id) != null).ToList();
        }
       */

    }
}
