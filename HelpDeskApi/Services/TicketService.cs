using HelpDeskApi.Data;
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
        public async Task<Ticket> CreatTicket(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<List<Ticket>> GetAll()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<Ticket> GetById(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            return ticket;
        }
    }
}
