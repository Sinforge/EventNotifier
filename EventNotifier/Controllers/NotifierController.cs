using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using System.Text.Json;

namespace EventNotifier.Controllers
{
    [ApiController]
    public class NotifierController : Controller
    {
        private readonly ILogger<NotifierController> _logger;
        private readonly IEventService _eventService;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        public NotifierController(ILogger<NotifierController> logger, IEventService eventService, IMapper mapper, IDistributedCache cache) {
            _eventService = eventService;
            _mapper = mapper;
            _logger = logger;
            _cache = cache;
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
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
                _logger.LogWarning(ex.Message);
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
                _logger.LogWarning(ex.Message);
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
                _logger.LogWarning(ex.Message);
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
                _logger.LogWarning(ex.Message);
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
        [ResponseCache(CacheProfileName = "Default60")]
        [ProducesResponseType(typeof(IEnumerable<ReadEventDTO>), 200)]
        public async Task<IEnumerable<ReadEventDTO>> GetAllEvents()
        {
            _logger.LogInformation("Getting all available events");
            string? allEventString = _cache != null ? await _cache.GetStringAsync("all") : null;
            if(allEventString != null)
            {
                _logger.LogInformation("Send cached events");
                return JsonSerializer.Deserialize<IEnumerable<ReadEventDTO>>(allEventString, _jsonSerializerOptions);
            }
            var events = _eventService.GetAllEvents();
            _logger.LogInformation("Send nocached events");
            var readEvents = from @event in events select _mapper.Map<ReadEventDTO>(@event);
            if(_cache != null) await _cache.SetStringAsync("all", JsonSerializer.Serialize(readEvents, _jsonSerializerOptions), new DistributedCacheEntryOptions{  AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)}) ;
            return readEvents;
        }

        [Route("/event/{id}")]
        [HttpGet]
        [ResponseCache(CacheProfileName = "Default60")]
        [ProducesResponseType(typeof(ReadEventDTO), 200)]
        public async Task<ReadEventDTO> GetEventById(int id)
        {
            _logger.LogInformation($"Send event with id {id} data");
            string? eventString = _cache != null? await _cache.GetStringAsync(id.ToString()) : null;
            if(eventString != null)
            {
                _logger.LogInformation($"Send cached event with id : {id}");
                return JsonSerializer.Deserialize<ReadEventDTO>(eventString, _jsonSerializerOptions);
            }

            _logger.LogInformation($"Send nocached event with id : {id}");
            var data = _mapper.Map<ReadEventDTO>(_eventService.GetEventById(id));
            if (_cache != null) await _cache.SetStringAsync(id.ToString(), JsonSerializer.Serialize(data, _jsonSerializerOptions), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
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
                    _logger.LogWarning(ex.Message);
                    return BadRequest(ex.Message);
                }
            }
            _logger.LogWarning("The user did not follow this event or such event does not exist");
            return BadRequest("The user did not follow this event or such event does not exist");
        }

        [Route("/recommendations")]
        [HttpGet]
        [Authorize]
        [ResponseCache(CacheProfileName = "Default60")]
        [ProducesResponseType(typeof(IEnumerable<ReadEventDTO>), 200)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ReadEventDTO>>> GetRecommendations()
        {
            _logger.LogInformation("Start of making recommendations");
            string? email = HttpContext?.User?.Identity?.Name;
            if (email != null)
            {
                try
                {
                    string? recommendationString = _cache != null ?await _cache.GetStringAsync(email) : null;
                    if(recommendationString != null)
                    {
                        _logger.LogInformation($"Send cached recommendations to user : {email}");
                        return Json(JsonSerializer.Deserialize<IEnumerable<ReadEventDTO>>(recommendationString, _jsonSerializerOptions));
                    }
                    _logger.LogInformation($"Send nocached events recommendation to user: {email}");
                    var recommmendations =  _eventService.GetEventsRecommendation(email);
                    var recommendationsToRead = from @event in recommmendations
                                                select _mapper.Map<ReadEventDTO>(@event);
                    if (_cache != null) await _cache.SetStringAsync(email, JsonSerializer.Serialize(recommendationsToRead, _jsonSerializerOptions), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                    }) ;
                    return Json(recommendationsToRead);
                }
                catch(Exception ex)
                {
                    _logger.LogWarning(ex.Message);
                    return BadRequest(ex.Message);
                }
            }
            return BadRequest("User not found");
        }
    }
    [NonController]
    public record PointToSearch(DTOs.Point point, double distance);
}
