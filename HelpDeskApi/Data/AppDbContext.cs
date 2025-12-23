using HelpDeskApi.Model;
using HelpDeskApi.Models;
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

        public DbSet<Ticket> Tickets { get; set; }

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

            modelBuilder.Entity<Ticket>()
                    .HasOne(t => t.CreatedBy)
                    .WithMany()
                    .HasForeignKey(t => t.CreatedById)
                    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.AssignedAgent)
                .WithMany()
                .HasForeignKey(t => t.AssignedAgentId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
