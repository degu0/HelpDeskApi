namespace HelpDeskApi.DTOs
{
    public class TicketGroupedDto
    {
        public List<ResponseTicketDto> AssignedToMe { get; set; } = [];
        public List<ResponseTicketDto> CreatedByMe { get; set; } = [];
        public List<ResponseTicketDto> FromToMyDepartment { get; set; } = [];
    }
}
