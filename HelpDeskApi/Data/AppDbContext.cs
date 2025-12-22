using HelpDeskApi.Model;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Department> Departments {get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Department>().HasData(
                    new Department { Id = 1, Name = "Finance" },
                    new Department { Id = 2, Name = "IT" },
                    new Department { Id = 3, Name = "Marketing" },
                    new Department { Id = 4, Name = "HumanResources" },
                    new Department { Id = 5, Name = "Administration" },
                    new Department { Id = 6, Name = "Legal" },
                    new Department { Id = 7, Name = "Logistics" },
                    new Department { Id = 8, Name = "Sales" }
                );
        }
    }
}
