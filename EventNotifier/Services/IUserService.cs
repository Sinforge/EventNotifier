using EventNotifier.DTOs;
using EventNotifier.Models;

namespace EventNotifier.Services
{
    public interface IUserService
    {
        public Task<bool> RegisterUser(CreateUserDTO createUserDTO);
        public User? CheckUserdata(string username, string password);
        public bool ConfirmEmail(string guid);

        public IEnumerable<User> GetUsers();


    }
}
