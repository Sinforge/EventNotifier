using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;

namespace EventNotifier.Controllers
{
    [ApiController]
    [ResponseCache(CacheProfileName = "Default60")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IOptions<Audience> _settings;
        public UserController(ILogger<UserController> logger, IUserService userService, IOptions<Audience> settings) {
            _logger = logger;
            _settings = settings;
            _userService = userService;
        }

        [Route("/registration")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registration([FromBody] CreateUserDTO userCreateDTO)
        {
            _logger.LogTrace("Try to create new user");
            bool isSuccessful = await _userService.RegisterUser(userCreateDTO);
            if (!isSuccessful)
            {
                return BadRequest("User with such email exist.Email must be unique");
            }
            return StatusCode(201, "We send message to your email to confirm your account.");
        }

        [Route("/confirm/{guid}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ConfirmEmail([FromRoute] string guid) {
            if(_userService.ConfirmEmail(guid))
            {
                return Ok("Email succesful confirmed");
            }
            return BadRequest("Incorrect request or email already confirmed");
        }
        [HttpGet("/login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login(string email, string password)
        {
            User? user = _userService.CheckUserdata(email, password);
            bool isCorrect = user != null;
            _logger.LogInformation("Checking user auth data");
            if (isCorrect)
            {
                _logger.LogInformation("User data is correct");

                // Set users claims
                _logger.LogInformation($"User has role : {user.Role.ToString()}");
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
                };

                // Create signing key
                var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Value.Secret));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateIssuer = true,
                    ValidIssuer = _settings.Value.Iss,
                    ValidateAudience = true,
                    ValidAudience = _settings.Value.Aud,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,

                };
                // Create JWT token
                var jwt = new JwtSecurityToken(
                    issuer: _settings.Value.Iss,
                    audience: _settings.Value.Aud,
                    claims: claims,
                    notBefore: DateTime.Now,
                    expires: DateTime.Now.Add(TimeSpan.FromHours(3)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                var encodedJWT = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJWT,
                    expires_in = (int)TimeSpan.FromHours(3).TotalSeconds
                };
                _logger.LogTrace($"Create jwt token: {encodedJWT}");
                return Json(response);
            }
            else
            {
                _logger.LogInformation("Incorrect user data");
                return BadRequest("Incorrect user data");
            }

        }


    }
    [NonController]
    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }

}
