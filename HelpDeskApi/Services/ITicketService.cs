using HelpDeskApi.Models;

namespace HelpDeskApi.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetAll();

        Task<Ticket> GetById(int id);

        Task<Ticket> CreatTicket(Ticket ticket);
    }
}
