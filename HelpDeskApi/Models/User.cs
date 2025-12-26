namespace HelpDeskApi.Model
{
    public class User
    {
        public int Id { get; set;  }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public int DepartmentId { get; set;  }
        public Department Department { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
