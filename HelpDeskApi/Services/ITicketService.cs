using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Models;

namespace HelpDeskApi.Services
{
    public interface ITicketService
    {
        Task<string> AssignTicket(int ticketId, int agentId);

        Task<Ticket> CreatTicket(CreateTicketDto dto, int id);

        Task<ResponseTicketDto> GetById(int id);

        Task<List<ResponseTicketDto>> GetByDepartment(int departmentId);

        Task<bool> GetConfirmationTicketByStatus(int ticketId, TicketStatusEnum status);

        Task<bool> GetConfirmationTicketByUser(int userId, int ticketId, TicketUserRelation userRelation);

        Task<int> GetDepartmentIdByTicket(int ticketId);

        Task<List<ResponseTicketDto>> GetTicketAssignedByUser(int agentId);

        Task<List<ResponseTicketDto>> GetTicketByStatus(TicketStatusEnum status, int userId);

        Task<TicketGroupedDto> GetTicketByUser(int userId, int departmentId);

        Task<List<ResponseTicketDto>> GetTicketCreatedByUser(int createdById);

        Task<PagedResponse<ResponseTicketPocketDto>> GetTicketPaged(int page, int pageSize);

        Task<bool> PatchStatus(TicketStatusEnum status, int ticketId);

        Task<string> TransferAssingTicket(int ticketId, int newAgentId);
    }
}
