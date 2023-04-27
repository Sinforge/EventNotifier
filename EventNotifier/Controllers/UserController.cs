using EventNotifier.DTOs;
using EventNotifier.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventNotifier.Controllers
{
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService) {
            _logger = logger;
            _userService = userService;
        }

        [Route("registration")]
        [HttpPost]
        public ActionResult Registration([FromBody] CreateUserDTO userCreateDTO)
        {
            _logger.LogTrace("Try to create new user");
            if(!_userService.RegisterUser(userCreateDTO))
            {
                return BadRequest("User with such email exist.Email must be unique");
            }
            return StatusCode(201);
        }

    }
}
