using EventNotifier.DTOs;
using EventNotifier.Models;
using NetTopologySuite.Geometries;

namespace EventNotifier.Services
{
    public interface IEventService
    {
        public void CreateEvent(CreateEventDTO createEventDTO);
        public void SubscribeToEvent(int eventId,  string userId);
        public void UnsubscribeToEvent(int eventId, string email);

        public Task SendNotification(int eventId, string timeLeft);

        public void RateEvent(int eventId, byte ratingNumber, string userEmail);
        public Event? GetEventById(int eventId);
        public IEnumerable<Event> GetAllEvents();

        public IEnumerable<Event> GetEventsRecommendation(string email);
        public IEnumerable<Event> GetEventByCoord(Coordinate coord, double distance);
        void DeleteEvent(int eventId);
        void UpdateEvent(Event @event);

        IEnumerable<Notification> GetUserNotifications(string email);
    }
}
