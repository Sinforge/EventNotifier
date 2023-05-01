using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            return Json(_mapper.Map<ReadEventDTO>(_eventService.GetEventById(id)));
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
    }
}
