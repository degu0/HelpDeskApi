using HelpDeskApi.Model;
using HelpDeskApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var department = await _service.GetById(id);
            if(department == null)
                return NotFound(new { mensagem = "Department não encontrado." });

            return Ok(department);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDepartment(Department department)
        {
            await _service.CreateDepartment(department);
            return CreatedAtAction(nameof(GetById), new { id = department.Id }, department);
        } 


    }
}
