using HelpDeskApi.Data;
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

        public async Task<ResponseTicketDto> GetById(int id)
        {
            return await _context.Tickets.
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
        }
    }
}
