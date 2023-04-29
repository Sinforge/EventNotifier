using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Repositories;

namespace EventNotifier.Services
{
    public class EventService : IEventService
    {
        private readonly ILogger<EventService> _logger;
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;
        public EventService(ILogger<EventService> logger, IEventRepo eventRepo, IMapper mapper, IUserRepo userRepo)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
            _eventRepo = eventRepo;
        }
        public bool CreateEvent(CreateEventDTO createEventDTO)
        {
            Event @event = _mapper.Map<Event>(createEventDTO);

            _logger.LogInformation("Creating event...");
            return _eventRepo.CreateEvent(@event);

        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _eventRepo.GetEvents();
        }

        public bool SubscribeToEvent(int eventId, string userEmail)
        {
            Event? @event = _eventRepo.GetEventById(eventId);
            User? user = _userRepo.GetUserByEmail(userEmail);
            if (@event != null && user != null)
            {
                @event.Subscribers.Add(user);
                _eventRepo.SaveChanges();
                return true;
            }
            return false;

        }
    }
}
