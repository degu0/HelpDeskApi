using HelpDeskApi.Model;

namespace HelpDeskApi.DTOs
{
    public class ResponseUserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string Department { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
