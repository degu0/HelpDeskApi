using HelpDeskApi.Data;
using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordService _passwordService;

        public AuthService(AppDbContext context, IPasswordService passwordService)
        {
            _context = context;
            _passwordService = passwordService;
        }

        public async Task<User?> Login(LoginUserDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return null;

            var senhaValida = _passwordService.VerifyPassword(
                dto.Password,
                user.PasswordHash
            );

            if (!senhaValida)
                return null;

            return user;
        }
    }

}
