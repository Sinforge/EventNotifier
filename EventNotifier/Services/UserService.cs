using AutoMapper;
using EventNotifier.DTOs;
using EventNotifier.Models;
using EventNotifier.Repositories;

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
        public bool CheckUserdata(string email, string password)
        {
            return _userRepo.GetUserByEmail(email)?.Password == password;
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
