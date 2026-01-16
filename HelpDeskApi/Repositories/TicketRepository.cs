using HelpDeskApi.Data;
using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Models;
using HelpDeskApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HelpDeskApi.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;    
        }

        public async Task<int> CountAsync()
        {
            return await _context.Tickets.CountAsync(); 
        }

        public async Task<Ticket> CreateAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> ExistsByStatusAsync(int ticketId, TicketStatusEnum status)
        {
            return await _context.Tickets.AnyAsync(ticket => ticket.Id == ticketId && ticket.Status == status);
        }

        public async Task<bool> ExistsCreatedByUserAsync(int userId, int ticketId)
        {
            return await _context.Tickets
                .AnyAsync(t => t.Id == ticketId && t.CreatedById == userId);
        }

        public async Task<bool> ExistsAssignedToUserAsync(int userId, int ticketId)
        {
            return await _context.Tickets
                .AnyAsync(t => t.Id == ticketId && t.AssignedAgentId == userId);
        }

        public async Task<List<Ticket>> GetAssignedToUserAsync(int agentId)
        {
            return await _context.Tickets
                .Where(ticket => ticket.AssignedAgentId == agentId)
                .Where(ticket => ticket.Status != TicketStatusEnum.Resolved &&
                                 ticket.Status != TicketStatusEnum.Closed &&
                                 ticket.Status != TicketStatusEnum.Archive)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Tickets
                .Where(ticket => ticket.DepartmentId == departmentId)
                .Where(ticket => ticket.Status != TicketStatusEnum.Resolved &&
                                 ticket.Status != TicketStatusEnum.Closed &&
                                 ticket.Status != TicketStatusEnum.Archive)
                .ToListAsync();
        }

        public async Task<Ticket?> GetByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Department)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedAgent)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetByStatusAsync(TicketStatusEnum status, int userId)
        {
            return await _context.Tickets
                .Where(ticket => ticket.CreatedById == userId)
                .Where(ticket => ticket.Status == status)
                .ToListAsync();
        }

        public async Task<List<Ticket>> GetCreatedByUserAsync(int userId)
        {
            return await _context.Tickets
                .Include(t => t.Department)
                .Include(t => t.CreatedBy)
                .Include(t => t.AssignedAgent)
                .Where(ticket => ticket.CreatedById == userId)
                .Where(ticket => ticket.Status != TicketStatusEnum.Resolved &&
                                 ticket.Status != TicketStatusEnum.Closed &&
                                 ticket.Status != TicketStatusEnum.Archive)
                .ToListAsync();
        }

        public async Task<int> GetDepartmentIdByTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
                return 0;

            return ticket.DepartmentId;
        }

        public async Task<List<Ticket>> GetPagedAsync(int page, int pageSize)
        {
            return await _context.Tickets
                        .OrderByDescending(t => t.CreatedAt)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
