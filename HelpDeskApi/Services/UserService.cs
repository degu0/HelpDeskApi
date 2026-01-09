using HelpDeskApi.Data;
using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;

namespace HelpDeskApi.Service
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;

        public UserService(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<User> CreatedUser(CreateUserDto dto)
        {
            var passwordHash = _passwordService.HashPassword(dto.Password);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = passwordHash,
                DepartmentId = dto.DepartmentId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<List<ResponseUserDto>> GetAll()
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

        public async Task<ResponseUserDto?> GetId(int id)
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

        public async Task<User> GetDepartmentByUser(int id)
        {
            return await _context.Users.FindAsync(id);
        }

    }
}
