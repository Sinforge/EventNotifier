using EventNotifier.DTOs;

namespace EventNotifier.Services
{
    public interface IUserService
    {
        public bool RegisterUser(CreateUserDTO createUserDTO);
        public bool CheckUserdata(string username, string password);
    }
}
