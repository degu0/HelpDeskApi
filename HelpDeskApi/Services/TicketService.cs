using HelpDeskApi.Data;
using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AssignTicket(int ticketId, int agentId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
                return "Chamado não encontrado.";

            if (ticket.AssignedAgentId is not null)
                return "Chamado ja foi atribuido.";

            ticket.Status = TicketStatusEnum.Seen;
            ticket.AssignedAgentId = agentId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return "Chamado foi atribuido com sucesso.";
        }

        public async Task<Ticket> CreatTicket(CreateTicketDto dto, int id)
        {
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                DepartmentId = dto.DepartmentId,
                CreatedById = id
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<List<ResponseTicketDto>> GetAll()
        {
            return await _context.Tickets.
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                })
                .ToListAsync();
        }

        public async Task<List<ResponseTicketDto>> GetByDepartment(int departmentId)
        {
            return await _context.Tickets.
                Where(ticket => ticket.DepartmentId == departmentId).
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                }).
                ToListAsync();
        }

        public async Task<ResponseTicketDto> GetById(int id)
        {
            var ticket = await _context.Tickets.
                Where(ticket => ticket.Id == id).
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                })
                .FirstOrDefaultAsync();

            return ticket == null ? throw new Exception() : ticket;
        }

        public async Task<int> GetDepartmentIdByTicket(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket is null)
                return 0;

            return ticket.DepartmentId;
        }
    }
}
