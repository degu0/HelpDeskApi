using HelpDeskApi.Data;
using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Models;
using Microsoft.EntityFrameworkCore;


namespace HelpDeskApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly AppDbContext _context;
        public TicketService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> AssignTicket(int ticketId, int agentId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket == null)
                return "Chamado não encontrado.";

            if (ticket.AssignedAgentId is not null)
                return "Chamado ja foi atribuido.";

            ticket.Status = TicketStatusEnum.Seen;
            ticket.AssignedAgentId = agentId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return "Chamado foi atribuido com sucesso.";
        }

        public async Task<Ticket> CreatTicket(CreateTicketDto dto, int id)
        {
            var ticket = new Ticket
            {
                Title = dto.Title,
                Description = dto.Description,
                DepartmentId = dto.DepartmentId,
                CreatedById = id
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<List<ResponseTicketDto>> GetAll()
        {
            return await _context.Tickets.
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                })
                .ToListAsync();
        }

        public async Task<List<ResponseTicketDto>> GetByDepartment(int departmentId)
        {
            return await _context.Tickets.
                Where(ticket => ticket.DepartmentId == departmentId).
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                }).
                ToListAsync();
        }

        public async Task<ResponseTicketDto> GetById(int id)
        {
            var ticket = await _context.Tickets.
                Where(ticket => ticket.Id == id).
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                })
                .FirstOrDefaultAsync();

            return ticket == null ? throw new Exception() : ticket;
        }
        public async Task<bool> GetConfirmationTicketByStatus(int userId, int ticketId, TicketStatusEnum status)
        {
            return await _context.Tickets.AnyAsync(t =>
                t.Id == ticketId &&
                t.Status == status
            );
        }

        public async Task<bool> GetConfirmationTicketByUser(
            int userId,
            int ticketId,
            string searchFor
        )
        {
            var query = _context.Tickets
                .Where(t => t.Id == ticketId);

            if (searchFor == "creat")
            {
                query = query.Where(t => t.CreatedById == userId);
            }
            else
            {
                query = query.Where(t => t.AssignedAgentId == userId);
            }

            return await query.AnyAsync();
        }


        public async Task<int> GetDepartmentIdByTicket(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket is null)
                return 0;

            return ticket.DepartmentId;
        }

        public async Task<List<ResponseTicketDto>> GetTicketAssignedByUser(int agentId)
        {
            return await _context.Tickets.
                Where(ticket => ticket.AssignedAgentId == agentId).
                Where(ticket => ticket.Status != TicketStatusEnum.Resolved && 
                                ticket.Status != TicketStatusEnum.Closed && 
                                ticket.Status != TicketStatusEnum.Archive).
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                }).
                ToListAsync();
        }

        public async Task<List<ResponseTicketDto>> GetTicketByStatus(TicketStatusEnum status, int userId)
        {
            return await _context.Tickets.
                Where(ticket => ticket.CreatedById == userId).
                Where(ticket => ticket.Status == status).
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                }).
                ToListAsync();
        }

        public async Task<TicketGroupedDto> GetTicketByUser(int userId, int departmentId)
        {
            var assignedToMe = await _context.Tickets
                                        .Where(ticket => ticket.AssignedAgentId == userId)
                                        .Where(ticket => ticket.Status != TicketStatusEnum.Resolved &&
                                               ticket.Status != TicketStatusEnum.Closed &&
                                               ticket.Status != TicketStatusEnum.Archive)
                                        .Select(ticket => new ResponseTicketDto
                                        {
                                            Id = ticket.Id,
                                            Title = ticket.Title,
                                            Description = ticket.Description,
                                            Status = ticket.Status.ToString(),
                                            Department = ticket.Department.Name,
                                            CreatedBy = ticket.CreatedBy.Name,
                                            AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                                            CreatedAt = ticket.CreatedAt,
                                            UpdatedAt = ticket.UpdatedAt,
                                        })
                                        .ToListAsync();

            var createdByMe = await _context.Tickets
                            .Where(ticket => ticket.CreatedById == userId)
                            .Where(ticket => ticket.Status != TicketStatusEnum.Resolved &&
                                               ticket.Status != TicketStatusEnum.Closed &&
                                               ticket.Status != TicketStatusEnum.Archive)
                            .Select(ticket => new ResponseTicketDto
                            {
                                Id = ticket.Id,
                                Title = ticket.Title,
                                Description = ticket.Description,
                                Status = ticket.Status.ToString(),
                                Department = ticket.Department.Name,
                                CreatedBy = ticket.CreatedBy.Name,
                                AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                                CreatedAt = ticket.CreatedAt,
                                UpdatedAt = ticket.UpdatedAt,
                            })
                            .ToListAsync();

            var fromToMyDepartment = await _context.Tickets
                                        .Where(ticket => ticket.DepartmentId == departmentId)
                                        .Where(ticket => ticket.Status != TicketStatusEnum.Resolved &&
                                               ticket.Status != TicketStatusEnum.Closed &&
                                               ticket.Status != TicketStatusEnum.Archive)
                                        .Select(ticket => new ResponseTicketDto
                                        {
                                            Id = ticket.Id,
                                            Title = ticket.Title,
                                            Description = ticket.Description,
                                            Status = ticket.Status.ToString(),
                                            Department = ticket.Department.Name,
                                            CreatedBy = ticket.CreatedBy.Name,
                                            AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                                            CreatedAt = ticket.CreatedAt,
                                            UpdatedAt = ticket.UpdatedAt,
                                        })
                                        .ToListAsync();

            return new TicketGroupedDto
            {
                AssignedToMe = assignedToMe,
                CreatedByMe = createdByMe,
                FromToMyDepartment = fromToMyDepartment
            };

        }

        public async Task<List<ResponseTicketDto>> GetTicketCreatedByUser(int createdById)
        {
            return await _context.Tickets.
                Where(ticket => ticket.CreatedById == createdById).
                Where(ticket => ticket.Status != TicketStatusEnum.Resolved && 
                                ticket.Status != TicketStatusEnum.Closed &&    
                                ticket.Status != TicketStatusEnum.Archive).
                Select(ticket => new ResponseTicketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description,
                    Status = ticket.Status.ToString(),
                    Department = ticket.Department.Name,
                    CreatedBy = ticket.CreatedBy.Name,
                    AssignedAgent = ticket.AssignedAgent != null ? ticket.AssignedAgent.Name : null,
                    CreatedAt = ticket.CreatedAt,
                    UpdatedAt = ticket.UpdatedAt,
                }).
                ToListAsync();
        }

        public async Task<bool> PatchStatus(TicketStatusEnum status, int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if (ticket is null)
                return false;

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> TransferAssingTicket(int ticketId, int newAgentId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);

            if(ticket == null)
                return "Chamado não encontrado.";

            if (ticket.AssignedAgentId is null)
                return "Chamado não foi atribuida ainda.";

            ticket.AssignedAgentId = newAgentId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return "Chamado foi transferido com sucesso.";
        }
    }
}
