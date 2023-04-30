using EventNotifier.DTOs;
using EventNotifier.Models;

namespace EventNotifier.Services
{
    public interface IEventService
    {
        public bool CreateEvent(CreateEventDTO createEventDTO);
        public bool SubscribeToEvent(int eventId,  string userId);
        public bool UnsubscribeToEvent(int eventId, string email);

        public Task SendNotification(int eventId, string timeLeft);

        public Event? GetEventById(int eventId);
        public IEnumerable<Event> GetAllEvents();
    }
}
