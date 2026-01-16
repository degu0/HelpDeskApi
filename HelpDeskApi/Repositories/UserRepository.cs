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

        public async Task<List<ResponseUserDto>> GetAllAsync()
        {
            return await _context.Users
            .Select(user => new ResponseUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Department = user.Department.Name,
                CreatedAt = user.CreatedAt,
            })
            .ToListAsync();
        }

        public async Task<int> GetDepartmentIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id); 

            return user != null ? user.DepartmentId : 0;
        }

        public async Task<ResponseUserDto> GetByIdAsync(int id)
        {
            return await _context.Users
                    .Where(user => user.Id == id)
                    .Select(user => new ResponseUserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Department = user.Department.Name,
                        CreatedAt = user.CreatedAt
                    })
                    .FirstOrDefaultAsync();
        }
    }
}
