using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Models;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketDto dto)
        {
            var ticket = await _service.CreatTicket(dto);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }
    }
}
