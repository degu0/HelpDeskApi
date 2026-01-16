using HelpDeskApi.Data;
using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User> CreateAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<int> GetDepartmentIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id); 

            return user != null ? user.DepartmentId : 0;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                    .Where(user => user.Id == id)
                    .FirstOrDefaultAsync();
        }
    }
}
