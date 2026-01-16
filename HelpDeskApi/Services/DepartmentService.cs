using HelpDeskApi.Model;
using HelpDeskApi.Repositories.Interfaces;

namespace HelpDeskApi.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Department> CreateDepartment(Department department)
        {
            return await _departmentRepository.CreateAsync(department);
        }

        public async Task<List<Department>> GetAll()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department> GetById(int id)
        {
            var department = await _departmentRepository.GetByIdAsync(id);

            return department ?? throw new Exception("Departamento não encontrado.");
        }
    }
}
