using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Models;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        public TicketController(ITicketService service)
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
            var ticket = await _service.GetById(id);
            if(ticket == null)
                return NotFound(new { mensagem = "Chamado não encontrado." });

            return Ok(ticket);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            ClaimsPrincipal currentUser = this.User;

            int id;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out id))
                return Unauthorized();

            var ticket = await _service.CreatTicket(dto,id);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }
    }
}
