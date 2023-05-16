using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;


namespace EventNotifier.Controllers
{
    public class NotifierController : Controller
    {
        private readonly ILogger<NotifierController> _logger;
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;


        public NotifierController(ILogger<NotifierController> logger, IEventService eventService, IMapper mapper) {
            _eventService = eventService;
            _mapper = mapper;
            _logger = logger;
     
       }

        [Route("/create-event")]
        [HttpPost]
        [Authorize(Roles= "Administration")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [Route("/delete-event")]
        [HttpPost]
        [Authorize(Roles ="Administration")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult DeleteEvent(int eventId)
        {
            _logger.LogInformation("Try to delete event...");
            try
            {
                _eventService.DeleteEvent(eventId);
                return StatusCode(204);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Route("/update-event")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateEvent([FromBody] Event @event)
        {
            _logger.LogInformation("Try to update event...");
            try
            {
                _eventService.UpdateEvent(@event);
                return StatusCode(204);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Route("/subscribe-to-event")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(typeof(IEnumerable<ReadEventDTO>), 200)]
        public IActionResult GetEventsByCoord([FromBody] PointToSearch pointToSearch) {
            _logger.LogInformation("Getting all event by coordinates");
            var coord = new Coordinate(pointToSearch.point.x, pointToSearch.point.y);
            var events = _eventService.GetEventByCoord(coord, pointToSearch.distance);
            _logger.LogInformation("Events succesful received");
            return Json(from @event in events select _mapper.Map<ReadEventDTO>(@event));
        }


        [Route("/all-events")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ReadEventDTO>), 200)]
        public IEnumerable<ReadEventDTO> GetAllEvents()
        {
            _logger.LogInformation("Getting all available events");
            var events = _eventService.GetAllEvents();
            _logger.LogInformation("Events successful received");
            var readEvents = from @event in events select _mapper.Map<ReadEventDTO>(@event);
            return readEvents;
        }

        [Route("/event/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(ReadEventDTO), 200)]
        public ReadEventDTO GetEventById(int id)
        {
            _logger.LogInformation($"Send event with id {id} data");
            var data = _mapper.Map<ReadEventDTO>(_eventService.GetEventById(id));
            return data;
        }

        [Route("/unsubscribe")]
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(IEnumerable<ReadEventDTO>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<ReadEventDTO>> GetRecommendations()
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
    [NonController]
    public record PointToSearch(DTOs.Point point, double distance);
}
