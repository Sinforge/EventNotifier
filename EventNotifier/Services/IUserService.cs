using EventNotifier.DTOs;
using EventNotifier.Models;

namespace EventNotifier.Services
{
    public interface IUserService
    {
        public bool RegisterUser(CreateUserDTO createUserDTO);
        public User? CheckUserdata(string username, string password);

    }
}
