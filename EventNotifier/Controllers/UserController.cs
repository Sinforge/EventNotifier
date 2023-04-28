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
        public ActionResult Registration([FromBody] CreateUserDTO userCreateDTO)
        {
            _logger.LogTrace("Try to create new user");
            if(!_userService.RegisterUser(userCreateDTO))
            {
                return BadRequest("User with such email exist.Email must be unique");
            }
            return StatusCode(201);
        }

        [HttpGet("/login")]
        public async Task<ActionResult> Login(string email, string password)
        {
            bool isCorrect = _userService.CheckUserdata(email, password);
            if (isCorrect)
            {
                // Set users claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, email),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, Role.DefaultUser.ToString())
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
                    expires: DateTime.Now.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

                var encodedJWT = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJWT,
                    expires_in = (int)TimeSpan.FromMinutes(2).TotalSeconds
                };
                return Json(response);
            }
            else
            {
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
