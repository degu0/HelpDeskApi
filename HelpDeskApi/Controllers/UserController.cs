using HelpDeskApi.Data;
using HelpDeskApi.Model;
using HelpDeskApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace HelpDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(int id)
        {
            var user = await _service.GetId(id);
            if (user == null)
                return NotFound(new { mensagem = "Usuario não encontrado." });

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreatedUser(User user)
        {
            if (!EmailValido(user.Email))
                return BadRequest(new { message = "Email invalido." });

            await _service.CreatedUser(user);

            return CreatedAtAction(nameof(GetId), new { id = user.Id }, user);

        }

        private bool EmailValido(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
