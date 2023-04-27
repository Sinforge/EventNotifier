using Microsoft.AspNetCore.Mvc;

namespace EventNotifier.Controllers
{
    public class NotifierController : Controller
    {
        private readonly ILogger<NotifierController> _logger;

        public NotifierController(ILogger<NotifierController> logger) {
            _logger = logger;
        
        }
    }
}
