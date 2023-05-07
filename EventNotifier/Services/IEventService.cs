using EventNotifier.DTOs;
using EventNotifier.Models;

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

        public IEnumerable<int> GetEventsRecommendation(string email);
    }
}
