using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Models;
using HelpDeskApi.Service;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;
using System.Security.Claims;

namespace HelpDeskApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _service;

        private readonly IUserService _userService;

        public TicketController(ITicketService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
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
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var id))
                return Unauthorized();

            var ticket = await _service.CreatTicket(dto,id);
            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }

        [Authorize]
        [HttpGet("department")]
        public async Task<IActionResult> GetByDepartment()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var id))
                return Unauthorized();

            var departmentId = await _userService.GetDepartmentByUser(id);

            return Ok(await _service.GetByDepartment(departmentId));
        }

        [Authorize]
        [HttpPatch("assign/{ticketId}")]
        public async Task<IActionResult> AssignTicket(int ticketId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userId, out var agentId))
                return Unauthorized();

            var userDepartmentId = await _userService.GetDepartmentByUser(agentId);
            var ticketDepartmentId = await _service.GetDepartmentIdByTicket(ticketId);

            if (userDepartmentId != ticketDepartmentId)
                return Unauthorized(new { mensagem = "O departamneto do usuario não é o mesmo do chamado." });

            var result = await _service.AssignTicket(ticketId, agentId);

            if (result != "Chamado foi atribuido com sucesso")
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [Authorize]
        [HttpGet("user/created")]
        public async Task<IActionResult> GetTicketCreatedByUser()
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(id, out var userId))
                return Unauthorized();

            var tickets = await _service.GetTicketCreatedByUser(userId);

            if (tickets is null)
                return Ok(new { mensagem = "A sua caixa de chamados esta vazio." });

            return Ok(tickets);
        }

        [Authorize]
        [HttpGet("user/assign")]
        public async Task<IActionResult> GetTicketAssignByUser()
        {
            string? id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(id, out var agentId))
                return Unauthorized();

            var tickets = await _service.GetTicketAssignedByUser(agentId);

            if(tickets is null)
                return Ok(new {mensagem = "A sua caixa de camados esta vazia."});

            return Ok(tickets);
        }

        [Authorize]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetTicketByStatus(TicketStatusEnum status)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var tickets = await _service.GetTicketByStatus(status, userId);

            if (tickets is null)
                return Ok(new { mensagem = "A sua caixa de camados esta vazia." });

            return Ok(tickets);
        }

        [Authorize]
        [HttpPatch("{ticketId}/status")]
        public async Task<IActionResult> PatchStatus([FromQuery] TicketStatusEnum status, int ticketId)
        {
            if(status != TicketStatusEnum.In_Progress && status != TicketStatusEnum.Resolved)
                return BadRequest(new {mensagem = "Status inválido." });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, "agent");

            if (!isValidTicket)
                return NotFound(new { mensagem = "Chamado não pertence ao usuario" });

            var updated = await _service.PatchStatus(status, ticketId);

            if(!updated)
                return BadRequest(new { mensagem = "Não foi possível alterar o status." });

            return Ok(new { mensagem = "Status alterado com sucesso." });
        }

        [Authorize]
        [HttpPatch("{ticketId}/closed")]
        public async Task<IActionResult> ClosedTicket(int ticketId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, "creat");

            if (!isValidTicket)
                return NotFound(new { mensagem = "Chamado não pertence ao usuario" });

            var updated = await _service.PatchStatus(TicketStatusEnum.Closed, ticketId);

            if (!updated)
                return BadRequest(new { mensagem = "Não foi possível alterar o status." });

            return Ok(new { mensagem = "Status fechado com sucesso." });
        }

        [Authorize]
        [HttpPatch("{ticketId}/reopen")]
        public async Task<IActionResult> ReopenTicket(int ticketId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, "creat");

            if (!isValidTicket)
                return NotFound(new { mensagem = "Chamado não pertence ao usuario" });

            var isValidStatusTicket = await _service.GetConfirmationTicketByStatus(userId, ticketId, TicketStatusEnum.Closed);

            if (!isValidStatusTicket)
                return NotFound(new { mensagem = "Chamado não esta fechado!" });

            var updated = await _service.PatchStatus(TicketStatusEnum.In_Progress, ticketId);

            if (!updated)
                return BadRequest(new { mensagem = "Não foi possível alterar o status." });

            return Ok(new { mensagem = "Status reaberto com sucesso." });
        }


        [Authorize]
        [HttpPatch("{ticketId}/archive")]
        public async Task<IActionResult> SoftDeleteTicket(int ticketId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, "creat");

            if (!isValidTicket)
                return NotFound(new { mensagem = "Chamado não pertence ao usuario" });

            var updated = await _service.PatchStatus(TicketStatusEnum.Archive, ticketId);

            if (!updated)
                return BadRequest(new { mensagem = "Não foi possível alterar o status." });

            return Ok(new { mensagem = "Status reaberto com sucesso." });
        }
    }
}
