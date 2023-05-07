using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;

namespace EventNotifier.Repositories
{
    public interface IEventRepo
    {
        public bool CreateEvent(Event @event);
        public IEnumerable<Event> GetEvents();
        public Event? GetEventById(int id);

        public void CreateNotification(Notification notification);
        public void ChangeToComplete(Event @event);
        public bool SaveChanges();
        public void ClearNotifications();
        public List<Rating> GetRatingsBySameEvent(User user);
       

    }
}
