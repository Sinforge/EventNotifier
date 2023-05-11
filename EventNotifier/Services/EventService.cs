using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Repositories;
using Hangfire;
using Microsoft.Extensions.Logging;
using NetTopologySuite.Geometries;

namespace EventNotifier.Services
{
    public class EventService : IEventService
    {
        private readonly ILogger<EventService> _logger;
        private readonly IEventRepo _eventRepo;
        private readonly IMapper _mapper;
        private readonly IUserRepo _userRepo;
        private readonly IEmailService _emailService;
        private readonly IRecommendationService _recommendationService;
        public EventService(ILogger<EventService> logger, IEventRepo eventRepo,
            IMapper mapper, IUserRepo userRepo, IEmailService emailService,
            IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
            _emailService = emailService;
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
            _eventRepo = eventRepo;
            // Clear notifications if users read them
            RecurringJob.AddOrUpdate("clear_notification",
                () => _eventRepo.ClearNotifications(), Cron.Daily);
        }
        public void CreateEvent(CreateEventDTO createEventDTO)
        {
            Event @event = _mapper.Map<Event>(createEventDTO);

            _logger.LogInformation("Creating event...");

            bool isSuccessfulCreated = _eventRepo.CreateEvent(@event) ;
            BackgroundJob.Enqueue(() => SendEventInfo(@event.Id, @event.Name, @event.Description));
            var timeLeft = @event.Date - DateTime.Now;


            if ( timeLeft.TotalDays >= 30)
            {
                BackgroundJob.Schedule(
                    () => SendNotification(@event.Id, "month"),
                    timeLeft.Subtract(TimeSpan.FromDays(30)));
            }
            if (timeLeft.TotalDays >= 7)
            {
                 BackgroundJob.Schedule(
                    () => SendNotification(@event.Id, "week"),
                    timeLeft.Subtract(TimeSpan.FromDays(7)));
            }
            if (timeLeft.TotalDays >= 1)
            {
                BackgroundJob.Schedule(
                    () => SendNotification(@event.Id, "day"),
                    timeLeft.Subtract(TimeSpan.FromDays(1)));
            }
            BackgroundJob.Schedule(
                    () => SendNotification(@event.Id, "5 hours"),
                    timeLeft.Subtract(TimeSpan.FromHours(5)));
            
            BackgroundJob.Schedule(
                       () => _eventRepo.ChangeToComplete(@event.Id), 
                       timeLeft.Add(TimeSpan.FromHours(5))
                    );

        }


        public IEnumerable<Event> GetAllEvents()
        {
            return _eventRepo.GetEvents();
        }

        public IEnumerable<Event> GetEventByCoord(Coordinate coord, double distance)
        {
            return _eventRepo.GetEventsByCoords(coord, distance);
        }

        public Event? GetEventById(int eventId)
        {
            return _eventRepo.GetEventById(eventId);
        }

        public IEnumerable<Event> GetEventsRecommendation(string email)
        {
            return _recommendationService.GetRecommendation(_userRepo.GetUserByEmail(email));
            
        }

        public void RateEvent(int eventId, byte ratingNumber, string userEmail)
        {
            Event @event = _eventRepo.GetEventById(eventId);
            if (@event == null)
            {
                throw new ArgumentException("Event with such id dont found");
            }
            @event.Ratings.Add(new Rating { RatingNumber = ratingNumber, User = _userRepo.GetUserByEmail(userEmail), Event = @event });
            _eventRepo.SaveChanges();
        }

        public async Task SendEventInfo(int evenId,string name, string description)
        {
            var users = _userRepo.GetAllUsers();
            foreach (var user in users)
            {
                if(user.EmailConfirmed)
                { 
                    await _emailService.SendMessageAsync(user.Email, name, $"A new event will be held in our city soon, all the details are below (check event description on our api <a>http://localhost:5146/event/{evenId} </a>):\n {description}");
                }
            }
        }


        public async Task SendNotification(int eventId, string timeLeft)
        {
            Event? @event = _eventRepo.GetEventById(eventId);
            if (@event != null)
            {
                var subscribers = @event?.Subscribers.ToList();
                string message = $"This notification was created so that you do not forget about the event you are following. {@event.Name} starts in {timeLeft}";
            
                foreach (var sub in subscribers)
                {
                    Notification notification = new Notification { HtmlText = message, Receiver = sub };
                    _eventRepo.CreateNotification(notification);
                    await _emailService.SendMessageAsync(sub.Email, $"{@event.Name} starts in {timeLeft}", message);
                }
                _eventRepo.SaveChanges();
            }
        }

        public void SubscribeToEvent(int eventId, string userEmail)
        {
            Event? @event = _eventRepo.GetEventById(eventId);
            User? user = _userRepo.GetUserByEmail(userEmail);
            if(@event.Subscribers.Contains(user))
            {
                throw new Exception("You already subscribe to this event");
            }
            if (@event.MaxSubscribers != 0 && @event.Subscribers.Count >= @event.MaxSubscribers)
            {
                throw new Exception("The event has reached the maximum number of subscribers");
            }
            if (@event == null || user == null)
            {
                throw new Exception("Incorrect user email or event id");

            }

            @event.Subscribers.Add(user);
            _eventRepo.SaveChanges();
        }

        public void UnsubscribeToEvent(int eventId, string email)
        {
            User? user = _userRepo.GetUserByEmail(email);
            Event? @event = _eventRepo?.GetEventById(eventId);
            if (user == null)
            {
                throw new Exception("User dont found");
            }
            if(@event == null)
            {
                throw new Exception("Event with such id dont exist");
            }
            if (!@event.Subscribers.Contains(user))
            {
                throw new Exception("You were not subscribed to this event");

            }
            if (@event != null && user != null)
            {
                @event.Subscribers.Remove(user);
                _eventRepo.SaveChanges();
            }
        }
    }
}
