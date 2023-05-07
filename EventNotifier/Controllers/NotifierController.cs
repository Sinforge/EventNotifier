using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Security.Claims;
using System.Text.Json;

namespace EventNotifier.Controllers
{
    public class NotifierController : Controller
    {
        private readonly ILogger<NotifierController> _logger;
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;
        private readonly IOptions<JsonOptions> _jsonOptions;
        private readonly NtsGeometryServices _ntsGeometryServices;

        public NotifierController(ILogger<NotifierController> logger, IEventService eventService, IMapper mapper,
            IOptions<JsonOptions> jsonOptions, NtsGeometryServices geometryServices) {
            _eventService = eventService;
            _mapper = mapper;
            _logger = logger;
            _ntsGeometryServices = geometryServices;
            _jsonOptions = jsonOptions;
        }


        [Route("/create-event")]
        [HttpPost]
        [Authorize(Roles= "Administration")]
        public IActionResult CreateEvent([FromBody] CreateEventDTO createEventDTO) 
        {
            _logger.LogInformation("Try to add event...");
            
            try
            {
                _eventService.CreateEvent(createEventDTO);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }
        [Route("/subscribe-to-event")]
        [HttpPost]
        [Authorize]
        public IActionResult SubscribeToEvent(int event_id)
        {
            _logger.LogInformation("Try subscribe to event");
            string? email= HttpContext?.User?.Identity?.Name;
            try
            {
                _eventService.SubscribeToEvent(event_id, email);
                return StatusCode(200);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("/rate-event")]
        [HttpPost]
        [Authorize]
        public IActionResult RateEvent(int eventId, byte ratingNumber) {
            _logger.LogInformation("start to rating event");
            string? email = HttpContext?.User?.Identity?.Name;
            if(email  == null)
            {
                return BadRequest("User dont found");
            }
            if(ratingNumber > 10 || ratingNumber < 0)
            {
                return BadRequest("Rating number must be positive and less or equals then 10");
            }
            try
            {
                _eventService.RateEvent(eventId, ratingNumber, email);
                return Ok("Event successful rated");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        

        }
        [Route("/all-events-by-coord")]
        [HttpGet]
        public IActionResult GetEventsByCoord(Coordinate coord, int distance) {
            _logger.LogInformation("Getting all event by coordinates");
            var events = _eventService.GetEventByCoord(coord, distance);
            _logger.LogInformation("Events succesful received");
            return Json(from @event in events select _mapper.Map<ReadEventDTO>(@event));
        }

        [Route("/all-events")]
        [HttpGet]
        public IActionResult GetAllEvents()
        {
            _logger.LogInformation("Getting all available events");
            var events = _eventService.GetAllEvents();
            _logger.LogInformation("Events successful received");
            return Json(from @event in events select _mapper.Map<ReadEventDTO>(@event)) ;
        }

        [Route("/event/{id}")]
        [HttpGet]
        public IActionResult GetEventById(int id)
        {
            _logger.LogInformation($"Send event with id {id} data");
            var data = _mapper.Map<ReadEventDTO>(_eventService.GetEventById(id));
            return Json(data);
        }

        [Route("/unsubscribe")]
        [HttpPost]
        [Authorize]
        public IActionResult UnsubscribeFromEvent(int eventId) {
            _logger.LogInformation("Try unsubscribe from event");
            string? email = HttpContext?.User?.Identity?.Name;
            if (email != null)
            {
                try
                {
                    _eventService.UnsubscribeToEvent(eventId, email);
                    return Ok();
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("The user did not follow this event or such event does not exist");
        }

        [Route("/recommendations")]
        [HttpGet]
        [Authorize]
        public IActionResult GetRecommendations()
        {
            _logger.LogInformation("Start of making recommendations");
            string? email = HttpContext?.User?.Identity?.Name;
            if (email != null)
            {
                try
                {
                    var recommmendations =  _eventService.GetEventsRecommendation(email);
                    return Json(from @event in recommmendations select _mapper.Map<ReadEventDTO>(@event));
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("User not found");
        }
    }
}
