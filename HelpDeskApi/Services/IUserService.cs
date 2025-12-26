using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApi.Service
{
    public interface IUserService
    {
        Task<List<User>> GetAll();
        Task<User> GetId(int id);
        Task<User> CreatedUser(CreateUserDto dto);
        Task<User?> Login(LoginUserDto dto);
    }
}
