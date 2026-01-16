using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApi.Service
{
    public interface IUserService
    {
        Task<User> CreatedUser(CreateUserDto dto);

        Task<List<ResponseUserDto>> GetAll();

        Task<int> GetDepartmentByUser(int id);

        Task<ResponseUserDto?> GetById(int id);
    }
}
