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
    public class UserController : IUserService
    {
        private readonly AppDbContext _context;
        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetId(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new { mensagem = "Contato não encontrado." });

            return Ok(user);
        }

        public async Task<IActionResult> CreatedUser(User user)
        {
            if (!EmailValido(user.Email))
                return BadRequest(new { message = "Email invalido." });

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtActionResult(nameof(GetId), new (id = user.Id), user);
        }

    }
}
