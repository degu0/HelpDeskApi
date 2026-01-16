using HelpDeskApi.DTOs;
using HelpDeskApi.Model;

namespace HelpDeskApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<List<ResponseUserDto>> GetAllAsync();
        Task<ResponseUserDto> GetByIdAsync(int id);
        Task<int> GetDepartmentIdAsync(int id);
    }
}
