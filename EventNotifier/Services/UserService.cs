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
        public UserService(IUserRepo userRepo, IMapper mapper) {
            _userRepo = userRepo;
            _mapper = mapper;
        }
        public User? CheckUserdata(string email, string password)
        {
            return _userRepo.GetUserByEmailAndPassword(email, password);
        }

        public bool RegisterUser(CreateUserDTO createUserDTO)
        {
            User user = _mapper.Map<User>(createUserDTO);
            if(_userRepo.GetUserByEmail(user.Email) != null)
            {
                return false;
            }
            _userRepo.CreateUser(user);
            return true;
        }

    }
}
