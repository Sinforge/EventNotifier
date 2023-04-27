using EventNotifier.Data;
using EventNotifier.Models;
using Microsoft.EntityFrameworkCore;

namespace EventNotifier.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDbContext _context;
        public UserRepo(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(_context));
        }


        public void ConfirmEmail(int userId)
        {
            User? user = GetUserById(userId);
            if (user != null) {
                user.EmailConfirmed = true;
                _context.SaveChanges();
            }
        }

        public void CreateUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void DeleteUser(int userId)
        {
            User? user = GetUserById(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChangesAsync();
            }
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(x => x.Email == email);
        }

        public User? GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public void SubscribeToEvent(int userId)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeToEvent(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
