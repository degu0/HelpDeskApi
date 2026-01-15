namespace HelpDeskApi.DTOs
{
    public class ResponseTicketPocketDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
