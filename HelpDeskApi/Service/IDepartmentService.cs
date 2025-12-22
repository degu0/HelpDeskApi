using HelpDeskApi.Model;

namespace HelpDeskApi.Service
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAll();

        Task<Department> GetById(int id);

        Task<Department> CreateDepartment(Department department);
    }
}
