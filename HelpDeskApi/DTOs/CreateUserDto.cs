namespace HelpDeskApi.DTOs
{
    public class CreateUserDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int DepartmentId { get; set; }
    }
}
