using HelpDeskApi.Domain.Enum;

namespace HelpDeskApi.Model
{
    public class User
    {
        public int Id { get; set;  }

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public int DepartmentId { get; set;  }
        public DepartmentEnum Department { get; set;  }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
