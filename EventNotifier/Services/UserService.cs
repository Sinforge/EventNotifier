using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Repositories;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace EventNotifier.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepo userRepo, IMapper mapper, IEmailService emailService, ILogger<UserService> logger)
        {
            _logger = logger;
            _userRepo = userRepo;
            _mapper = mapper;
            _emailService = emailService;
        }
        public User? CheckUserdata(string email, string password)
        {
            return _userRepo.GetUserByEmailAndPassword(email, password);
        }

        public bool ConfirmEmail(string guid)
        {
            if(_userRepo.ConfirmEmail(guid))
            {
                _logger.LogInformation("User successful confirm email");
                return true;
            }
            _logger.LogInformation("Not correct guid or email already confirmed");
            return false;
        }

        public IEnumerable<User> GetUsers()
        {
            return _userRepo.GetAllUsers();
        }

        public async Task<bool> RegisterUser(CreateUserDTO createUserDTO)
        {
            User user = _mapper.Map<User>(createUserDTO);
            if(_userRepo.GetUserByEmail(user.Email) != null)
            {
                return false;
            }
            user.ConfirmCode = Guid.NewGuid().ToString();
            await _emailService.SendMessageAsync
                (user.Email, 
                "Confirm your email",
                $"<h3>Confirm email to receive event notifications</h3>" +
                $"<p>Visit next link to confirm email: <a>http://localhost:5146/confirm/{user.ConfirmCode}</a></p>" );
            _userRepo.CreateUser(user);

            return true;
        }


    }
}
