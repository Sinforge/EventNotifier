using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace EventNotifier.Services
{
    public class EventService : IEventService
    {
        private readonly ILogger<EventService> _logger;
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;
        private readonly IEmailService _emailService;
        public EventService(ILogger<EventService> logger, IEventRepo eventRepo, IMapper mapper, IUserRepo userRepo, IEmailService emailService)
        {
            _emailService = emailService;
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
            _eventRepo = eventRepo;
        }
        public bool CreateEvent(CreateEventDTO createEventDTO)
        {
            Event @event = _mapper.Map<Event>(createEventDTO);

            _logger.LogInformation("Creating event...");

            bool isSuccessfulCreated = _eventRepo.CreateEvent(@event);
            BackgroundJob.Enqueue(() => SendEventInfo(@event.Id, @event.Name, @event.Description));
            var timeLeft = @event.Date - DateTime.Now;


            if ( timeLeft.TotalDays >= 30)
            {
                string jobId1 = BackgroundJob.Schedule(
                    () => SendNotification(@event.Id, "month"),
                    timeLeft.Subtract(TimeSpan.FromDays(30)));
            }
            if (timeLeft.TotalDays >= 7)
            {
                string jobId1 = BackgroundJob.Schedule(
                    () => SendNotification(@event.Id, "week"),
                    timeLeft.Subtract(TimeSpan.FromDays(7)));
            }
            if (timeLeft.TotalDays >= 1)
            {
                string jobId1 = BackgroundJob.Schedule(
                    () => SendNotification(@event.Id, "day"),
                    timeLeft.Subtract(TimeSpan.FromDays(1)));
            }
            return isSuccessfulCreated;

        }


        public IEnumerable<Event> GetAllEvents()
        {
            return _eventRepo.GetEvents();
        }

        public Event? GetEventById(int eventId)
        {
            return _eventRepo.GetEventById(eventId);
        }

        public async Task SendEventInfo(int evenId,string name, string description)
        {
            var users = _userRepo.GetAllUsers();
            foreach (var user in users)
            {
                if(user.EmailConfirmed)
                { 
                    await _emailService.SendMessageAsync(user.Email, name, $"A new event will be held in our city soon, all the details are below (check event description on our api <a>http://localhost:5146/event/{evenId}):\n {description}");
                }
            }
        }


        public async Task SendNotification(int eventId, string timeLeft)
        {
            Event? @event = _eventRepo.GetEventById(eventId);
            if (@event != null)
            {
                var subscribers = @event?.Subscribers.ToList();
                foreach(var sub in subscribers)
                {
                    await _emailService.SendMessageAsync(sub.Email, $"{@event.Name} starts in {timeLeft}",
                        "This notification was created so that you do not forget about the event you are following.");
                }
            }
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

        public bool UnsubscribeToEvent(int eventId, string email)
        {
            User? user = _userRepo.GetUserByEmail(email);
            Event? @event = _eventRepo?.GetEventById(eventId);
            if (@event != null && user != null)
            {
                @event.Subscribers.Remove(user);
                _eventRepo.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
