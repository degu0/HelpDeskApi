using HelpDeskApi.Domain.Enum;
using HelpDeskApi.Model;

namespace HelpDeskApi.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TicketStatusEnum Status { get; set; } = TicketStatusEnum.Open;

        public int DepartmentId { get; set; }
        public Department Department { get; set; } = null!;

        public int CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public int? AssignedAgentId { get; set; }
        public User? AssignedAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
