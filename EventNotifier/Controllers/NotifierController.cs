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

        public NotifierController(ILogger<NotifierController> logger, IEventService eventService) {
            _eventService = eventService;
            _logger = logger;
        }


        [Route("/create-event")]
        [HttpPost]
        [Authorize(Roles= "Administration")]
        public IActionResult CreateEvent([FromBody] CreateEventDTO createEventDTO) 
        {
            _logger.LogInformation("Try to add event...");
            
            if(_eventService.CreateEvent(createEventDTO))
            {

                return StatusCode(201);
            }
            return BadRequest("Something is going wrong");

        }
        [Route("/subscribe-to-event")]
        [HttpPost]
        [Authorize]
        public IActionResult SubscribeToEvent(int event_id)
        {
            string? email= HttpContext?.User?.Identity?.Name;
            if (_eventService.SubscribeToEvent(event_id, email))
            {
                return StatusCode(200);
            }
            return BadRequest("We cant found event with such id");
        }

        [Route("/all-events")]
        [HttpGet]
        public IActionResult GetAllEvents()
        {
            _logger.LogInformation("Getting all available events");
            var events = _eventService.GetAllEvents();
            _logger.LogInformation("Events successful received");
            return Json(events);
        }

        [Route("/event/{id}")]
        [HttpGet]
        public IActionResult GetEventById(int id)
        {
            return Json(_eventService.GetEventById(id));
        }

        [Route("/unsubscribe")]
        [HttpPost]
        [Authorize]
        public IActionResult UnsubscribeFromEvent(int eventId) {
            string? email = HttpContext?.User?.Identity?.Name;
            if (email != null && _eventService.UnsubscribeToEvent(eventId, email))
            {
                return Ok();
            }
            return BadRequest("The user did not follow this event or such event does not exist");
        }
    }
}
