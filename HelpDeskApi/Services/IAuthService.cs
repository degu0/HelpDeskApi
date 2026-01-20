using HelpDeskApi.DTOs;
using HelpDeskApi.Model;

namespace HelpDeskApi.Services
{
    public interface IAuthService
    {
        Task<User?> Login(LoginUserDto dto);
    }
}
