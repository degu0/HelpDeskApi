using HelpDeskApi.DTOs;
using HelpDeskApi.Models;

namespace HelpDeskApi.Services
{
    public interface ITicketService
    {
        Task<List<ResponseTicketDto>> GetAll();

        Task<ResponseTicketDto> GetById(int id);

        Task<Ticket> CreatTicket(CreateTicketDto dto, int id);

        Task<List<ResponseTicketDto>> GetByDepartment(int departmentId);
    }
}
