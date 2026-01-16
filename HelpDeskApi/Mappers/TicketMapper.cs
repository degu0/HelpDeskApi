using HelpDeskApi.DTOs;
using HelpDeskApi.Models;

namespace HelpDeskApi.Mappers
{
    public class TicketMapper
    {
        public static ResponseTicketDto ToResponseDto(Ticket ticket)
        {
            return new ResponseTicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Department = ticket.Department?.Name,
                CreatedBy = ticket.CreatedBy?.Name,
                AssignedAgent = ticket.AssignedAgent?.Name,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
            };
        }
    }
}
