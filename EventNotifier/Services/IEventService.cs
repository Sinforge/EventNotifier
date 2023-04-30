using EventNotifier.DTOs;
using EventNotifier.Models;

namespace EventNotifier.Services
{
    public interface IEventService
    {
        public bool CreateEvent(CreateEventDTO createEventDTO);
        public bool SubscribeToEvent(int eventId,  string userId);

        public Task SendNotification(int eventId, string timeLeft);

        public IEnumerable<Event> GetAllEvents();
    }
}
