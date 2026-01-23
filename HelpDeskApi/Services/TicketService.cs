using HelpDeskApi.Data;
using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Mappers;
using HelpDeskApi.Models;
using HelpDeskApi.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace HelpDeskApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        public TicketService(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<string> AssignTicket(int ticketId, int agentId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                return "Chamado não encontrado.";

            if (ticket.AssignedAgentId is not null)
                return "Chamado ja foi atribuido.";

            ticket.Status = TicketStatusEnum.Seen;
            ticket.AssignedAgentId = agentId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.SaveChangesAsync();
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

            return await _ticketRepository.CreateAsync(ticket);
        }

        public async Task<List<ResponseTicketDto>> GetByDepartment(int departmentId)
        {
            var tickets = await _ticketRepository.GetByDepartmentIdAsync(departmentId);

            return tickets.Select(TicketMapper.ToResponseDto).ToList();
        }

        public async Task<ResponseTicketDto> GetById(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);

            if (ticket == null)
                throw new Exception("Ticket não encontrado");

            return new ResponseTicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Status = ticket.Status.ToString(),
                Department = ticket.Department.Name,
                CreatedBy = ticket.CreatedBy.Name,
                AssignedAgent = ticket.AssignedAgent?.Name,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
            };
        }
        public async Task<bool> GetConfirmationTicketByStatus(int ticketId, TicketStatusEnum status)
        {
            return await _ticketRepository.ExistsByStatusAsync(ticketId, status);
        }

        public async Task<bool> GetConfirmationTicketByUser(int userId, int ticketId, TicketUserRelation userRelation)
        {
            return userRelation switch
            {
                TicketUserRelation.CreatedBy =>
                    await _ticketRepository.ExistsCreatedByUserAsync(ticketId, userId),

                TicketUserRelation.AssignedTo =>
                    await _ticketRepository.ExistsAssignedToUserAsync(ticketId, userId),

                _ => false
            };
        }

        public async Task<int> GetDepartmentIdByTicket(int ticketId)
        {
            return await _ticketRepository.GetDepartmentIdByTicketAsync(ticketId);
        }

        public async Task<List<ResponseTicketDto>> GetTicketAssignedByUser(int agentId)
        {
            var tickets = await _ticketRepository.GetAssignedToUserAsync(agentId);

            return tickets.Select(TicketMapper.ToResponseDto).ToList();
        }

        public async Task<List<ResponseTicketDto>> GetTicketByStatus(TicketStatusEnum status, int userId)
        {
            var tickets = await _ticketRepository.GetByStatusAsync(status, userId);

            return tickets.Select(TicketMapper.ToResponseDto).ToList();
        }

        public async Task<TicketGroupedDto> GetTicketByUser(int userId, int departmentId)
        {
            var assignedToMe = await _ticketRepository.GetAssignedToUserAsync(userId);

            var createdByMe = await _ticketRepository.GetCreatedByUserAsync(userId);

            var fromToMyDepartment = await _ticketRepository.GetByDepartmentIdAsync(departmentId);

            return new TicketGroupedDto
            {
                AssignedToMe = assignedToMe.Select(TicketMapper.ToResponseDto).ToList(),
                CreatedByMe = createdByMe.Select(TicketMapper.ToResponseDto).ToList(),
                FromToMyDepartment = fromToMyDepartment.Select(TicketMapper.ToResponseDto).ToList()
            };

        }

        public async Task<List<ResponseTicketDto>> GetTicketCreatedByUser(int createdById)
        {
            var tickets = await _ticketRepository.GetCreatedByUserAsync(createdById);

            return tickets.Select(TicketMapper.ToResponseDto).ToList();
        }

        public async Task<PagedResponse<ResponseTicketPocketDto>> GetTicketPaged(int page, int pageSize)
        {
            var totalItems = await _ticketRepository.CountAsync();

            var tickets = await _ticketRepository.GetPagedAsync(page, pageSize);

            var items = tickets.Select(ticket => new ResponseTicketPocketDto
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Status = ticket.Status.ToString(),
                    CreatedAt = ticket.CreatedAt
                }).ToList();


            return new PagedResponse<ResponseTicketPocketDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                Items = items
            };
        }

        public async Task<bool> PatchStatus(TicketStatusEnum status, int ticketId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket is null)
                return false;

            ticket.Status = status;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.SaveChangesAsync();
            return true;
        }

        public async Task<string> TransferAssingTicket(int ticketId, int newAgentId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);

            if (ticket == null)
                return "Chamado não encontrado.";

            if (ticket.AssignedAgentId == null)
                return "Chamado não foi atribuida ainda.";

            ticket.AssignedAgentId = newAgentId;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _ticketRepository.SaveChangesAsync();
            return "Chamado foi transferido com sucesso.";
        }
    }
}
