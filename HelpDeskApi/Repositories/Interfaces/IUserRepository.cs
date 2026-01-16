using HelpDeskApi.DTOs;
using HelpDeskApi.Model;

namespace HelpDeskApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<int> GetDepartmentIdAsync(int id);
    }
}
