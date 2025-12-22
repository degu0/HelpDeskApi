using HelpDeskApi.Data;
using HelpDeskApi.Model;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreatedUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetId(int id)
        {
            var user = await _context.Users.FindAsync(id);
            return user;
        }
    }
}
