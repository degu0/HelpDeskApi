using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Service;
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

        private readonly IUserService _userService;

        public TicketController(ITicketService service, IUserService userService)
        {
            _service = service;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest();

            var result = await _service.GetTicketPaged(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _service.GetById(id);
            if(ticket == null)
                return NotFound(new { message = "Chamado não encontrado." });

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
                return Unauthorized(new { message = "O departamento do usuario não é o mesmo do chamado." });

            var result = await _service.AssignTicket(ticketId, agentId);

            if (result != "Chamado foi atribuido com sucesso.")
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
                return Ok(new { message = "A sua caixa de chamados esta vazia." });

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
                return Ok(new { message = "A sua caixa de chamados esta vazia."});

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
                return Ok(new { message = "A sua caixa de chamados esta vazia." });

            return Ok(tickets);
        }

        [Authorize]
        [HttpPatch("{ticketId}/status")]
        public async Task<IActionResult> PatchStatus([FromQuery] TicketStatusEnum status, int ticketId)
        {
            if(status != TicketStatusEnum.In_Progress && status != TicketStatusEnum.Resolved)
                return BadRequest(new { message = "Status inválido." });

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, TicketUserRelation.AssignedTo);

            if (!isValidTicket)
                return NotFound(new { message = "Chamado não pertence ao usuario" });

            var updated = await _service.PatchStatus(status, ticketId);

            if(!updated)
                return BadRequest(new { message = "Não foi possível alterar o status." });

            return Ok(new { message = "Status alterado com sucesso." });
        }

        [Authorize]
        [HttpPatch("{ticketId}/closed")]
        public async Task<IActionResult> ClosedTicket(int ticketId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, TicketUserRelation.CreatedBy);

            if (!isValidTicket)
                return NotFound(new { message = "Chamado não pertence ao usuario" });

            var updated = await _service.PatchStatus(TicketStatusEnum.Closed, ticketId);

            if (!updated)
                return BadRequest(new { message = "Não foi possível alterar o status." });

            return Ok(new { message = "Status fechado com sucesso." });
        }

        [Authorize]
        [HttpPatch("{ticketId}/reopen")]
        public async Task<IActionResult> ReopenTicket(int ticketId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, TicketUserRelation.CreatedBy);

            if (!isValidTicket)
                return NotFound(new { message = "Chamado não pertence ao usuario" });

            var isValidStatusTicket = await _service.GetConfirmationTicketByStatus(ticketId, TicketStatusEnum.Closed);

            if (!isValidStatusTicket)
                return NotFound(new { message = "Chamado não esta fechado!" });

            var updated = await _service.PatchStatus(TicketStatusEnum.In_Progress, ticketId);

            if (!updated)
                return BadRequest(new { message = "Não foi possível alterar o status." });

            return Ok(new { message = "Status reaberto com sucesso." });
        }


        [Authorize]
        [HttpPatch("{ticketId}/archive")]
        public async Task<IActionResult> SoftDeleteTicket(int ticketId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isValidTicket = await _service.GetConfirmationTicketByUser(userId, ticketId, TicketUserRelation.CreatedBy);

            if (!isValidTicket)
                return NotFound(new { message = "Chamado não pertence ao usuario" });

            var updated = await _service.PatchStatus(TicketStatusEnum.Archive, ticketId);

            if (!updated)
                return BadRequest(new { message = "Não foi possível alterar o status." });

            return Ok(new { message = "Status reaberto com sucesso." });
        }

        [Authorize]
        [HttpPatch("{ticketId}/transfer/{newAgentId}")]
        public async Task<IActionResult> TransferTicket(int ticketId, int newAgentId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var agentId))
                return Unauthorized();

            var userDepartmentId = await _userService.GetDepartmentByUser(agentId);
            var ticketDepartmentId = await _service.GetDepartmentIdByTicket(ticketId);

            if (userDepartmentId != ticketDepartmentId)
                return Unauthorized(new { message = "O departamento do usuario não é o mesmo do chamado." });

            var result = await _service.TransferAssingTicket(ticketId, newAgentId);

            if (result != "Chamado foi transferido com sucesso.")
                return BadRequest(new { message = result });

            return Ok(new { message = result });
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetAllTicketByUser()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var departmentId = await _userService.GetDepartmentByUser(userId);

            return Ok(await _service.GetTicketByUser(userId, departmentId));
        }
    }
}
