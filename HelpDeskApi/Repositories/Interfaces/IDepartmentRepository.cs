using HelpDeskApi.Model;

namespace HelpDeskApi.Repositories.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<Department> CreateAsync(Department department);
        Task<List<Department>> GetAllAsync();
        Task<Department> GetByIdAsync(int id);
    }
}
