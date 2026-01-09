using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApi.Service
{
    public interface IUserService
    {
        Task<List<ResponseUserDto>> GetAll();
        Task<ResponseUserDto?> GetId(int id);

        Task<User> GetDepartmentByUser(int id);
        Task<User> CreatedUser(CreateUserDto dto);
    }
}
