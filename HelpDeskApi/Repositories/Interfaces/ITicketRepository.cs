using HelpDeskApi.Domain.Enum;
using HelpDeskApi.Models;

namespace HelpDeskApi.Repositories.Interfaces
{
    public interface ITicketRepository
    {
        Task<int> CountAsync();

        Task<Ticket> CreateAsync(Ticket ticket);

        Task<bool> ExistsByStatusAsync(int ticketId, TicketStatusEnum status);

        Task<bool> ExistsCreatedByUserAsync(int userId, int ticketId);

        Task<bool> ExistsAssignedToUserAsync(int userId, int ticketId);

        Task<List<Ticket>> GetAssignedToUserAsync(int agentId);

        Task<List<Ticket>> GetByDepartmentIdAsync(int departmentId);

        Task<Ticket?> GetByIdAsync(int id);

        Task<List<Ticket>> GetByStatusAsync(TicketStatusEnum status, int userId);

        Task<List<Ticket>> GetCreatedByUserAsync(int userId);

        Task<int> GetDepartmentIdByTicketAsync(int ticketId);

        Task<List<Ticket>> GetPagedAsync(int page, int pageSize);

        Task SaveChangesAsync();
    }
}
