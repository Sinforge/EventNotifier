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


        public bool ConfirmEmail(string guid)
        {
            User? user = _context.Users.FirstOrDefault(u => u.ConfirmCode == guid);
            if (user != null) {
                user.EmailConfirmed = true;
                user.ConfirmCode = null;
                _context.SaveChanges();
                return true;
            }
            return false;
            
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

        public IEnumerable<User> GetAllUsers()
        {
            return _context.Users.ToList();
        }

        public User? GetUserByEmail(string email)
        {
            return _context.Users.Include(u => u.Ratings).FirstOrDefault(x => x.Email == email);
        }
  
        public User? GetUserByEmailAndPassword(string email, string password)
        {
            return _context.Users.FirstOrDefault(x => x.Email == email && x.Password == password);
        }

        public User? GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }


    }
}
