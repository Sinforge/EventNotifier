using EventNotifier.Data;
using EventNotifier.Models;

namespace EventNotifier.Repositories
{
    public interface IUserRepo
    {
        public void CreateUser(User user);

        public User? GetUserById(int userId);
        public User? GetUserByEmail(string email);
        public User? GetUserByEmailAndPassword(string email, string password);
        public void ConfirmEmail(int userId);
        public void DeleteUser(int userId);

        public void SubscribeToEvent(User user, Event @event);
        public void UnsubscribeToEvent(int userId);

    }
}
