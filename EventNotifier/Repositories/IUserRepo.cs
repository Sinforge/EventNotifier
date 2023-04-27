using EventNotifier.Data;
using EventNotifier.Models;

namespace EventNotifier.Repositories
{
    public interface IUserRepo
    {
        public void CreateUser(User user);

        public User? GetUserById(int userId);
        public User? GetUserByEmail(string email);
        public void ConfirmEmail(int userId);
        public void DeleteUser(int userId);

        public void SubscribeToEvent(int userId);
        public void UnsubscribeToEvent(int userId);

    }
}
