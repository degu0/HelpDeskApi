using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Models;

namespace HelpDeskApi.Services
{
    public interface ITicketService
    {
        Task<ResponseTicketDto> GetById(int id);

        Task<Ticket> CreatTicket(CreateTicketDto dto, int id);

        Task<List<ResponseTicketDto>> GetByDepartment(int departmentId);

        Task<string> AssignTicket(int ticketId, int agentId);

        Task<int> GetDepartmentIdByTicket(int ticketId);

        Task<List<ResponseTicketDto>> GetTicketCreatedByUser(int createdById);

        Task<List<ResponseTicketDto>> GetTicketAssignedByUser(int agentId);

        Task<List<ResponseTicketDto>> GetTicketByStatus(TicketStatusEnum status, int userId);

        Task<TicketGroupedDto> GetTicketByUser(int userId, int departmentId);

        Task<bool> PatchStatus(TicketStatusEnum status, int ticketId);

        Task<bool> GetConfirmationTicketByUser(int userId, int ticketId, string searchFor);

        Task<bool> GetConfirmationTicketByStatus(int userId, int ticketId, TicketStatusEnum status);

        Task<string> TransferAssingTicket(int ticketId, int newAgentId);

        Task<PagedResponse<ResponseTicketPocketDto>> GetTicketPaged(int page, int pageSize);
    }
}
