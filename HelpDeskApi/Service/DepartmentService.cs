using HelpDeskApi.Data;
using HelpDeskApi.Model;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _context;
        public DepartmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Department> CreateDepartment(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<List<Department>> GetAll()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department> GetById(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            return department;
        }
    }
}
