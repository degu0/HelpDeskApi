using HelpDeskApi.Domain.Enum;

namespace HelpDeskApi.DTOs
{
    public class ResponseTicketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public string? AssignedAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
