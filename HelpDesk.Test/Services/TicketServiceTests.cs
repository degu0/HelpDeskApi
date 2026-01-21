using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Mappers;
using HelpDeskApi.Model;
using HelpDeskApi.Models;
using HelpDeskApi.Repositories.Interfaces;
using HelpDeskApi.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace HelpDeskApi.Tests.Services
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _repositoryMock;
        private readonly TicketService _service;

        public TicketServiceTests()
        {
            _repositoryMock = new Mock<ITicketRepository>();
            _service = new TicketService( _repositoryMock.Object );
        }

        [Fact]
        public async Task AssignTicket_ShouldAssignAgentAndReturnSuccessMessage()
        {
            var ticket = new Ticket
            {
                Id = 1,
                AssignedAgentId = null,
                Status = TicketStatusEnum.Open,
                UpdatedAt = DateTime.UtcNow,
            };

            int agentId = 2;

            _repositoryMock
                .Setup(r => r.GetByIdAsync(ticket.Id))
                .ReturnsAsync(ticket);

            var result = await _service.AssignTicket(ticket.Id, agentId);

            Assert.Equal("Chamado foi atribuido com sucesso.", result);

            Assert.Equal(agentId, ticket.AssignedAgentId);
            Assert.Equal(TicketStatusEnum.Seen, ticket.Status);

            _repositoryMock.Verify(
                r => r.SaveChangesAsync(),
                Times.Once
            );

        }

        [Fact]
        public async Task AssignTicket_ShouldReturnNotFoundMessage()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Ticket?)null);

            var result = await _service.AssignTicket(99, 10);

            Assert.Equal("Chamado não encontrado.", result);
        }

        [Fact]
        public async Task AssignTicket_ShouldReturnAlreadyAssignedMessage_WhenTicketIsAssigned()
        {
            var ticket = new Ticket
            {
                Id = 1,
                AssignedAgentId = 10,
                Status = TicketStatusEnum.Open,
                UpdatedAt = DateTime.UtcNow,
            };

            int agentId = 2;

            _repositoryMock
                .Setup(r => r.GetByIdAsync(ticket.Id))
                .ReturnsAsync(ticket);

            var result = await _service.AssignTicket(ticket.Id, agentId);

            Assert.Equal("Chamado ja foi atribuido.", result);
        }

        [Fact]
        public async Task CreatTicket_ShouldSalveAndReturnTicket()
        {
            var ticket = new CreateTicketDto
            {
                Title = "Title test",
                Description = "Description test",
                DepartmentId = 1,
            };

            var userId = 10;

            _repositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket t) => t);

            var result = await _service.CreatTicket(ticket, userId);

            Assert.NotNull(result);
            Assert.Equal(ticket.Title, result.Title);
            Assert.Equal(ticket.Description, result.Description);
            Assert.Equal(ticket.DepartmentId, result.DepartmentId);

            _repositoryMock.Verify(
                r => r.CreateAsync(It.IsAny<Ticket>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetByDepartment_ShouldReturnMappedTickets()
        {
            int departmentId = 1;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    Title = "Erro no sistema",
                    DepartmentId = departmentId,
                    Status = TicketStatusEnum.Open
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Problema de login",
                    DepartmentId = departmentId,
                    Status = TicketStatusEnum.Seen
                }
            };

            _repositoryMock
                .Setup(r => r.GetByDepartmentIdAsync(departmentId))
                .ReturnsAsync(tickets);

            var result = await _service.GetByDepartment(departmentId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.All(result, dto =>
            {
                Assert.IsType<ResponseTicketDto>(dto);
            });

            Assert.Equal(tickets[0].Id, result[0].Id);
            Assert.Equal(tickets[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetById_ShouldReturnDto_WhenTicketExists()
        {
            var ticket = new Ticket
            {
                Id = 1,
                Title = "Erro no sistema",
                Description = "Sistema fora do ar",
                Status = TicketStatusEnum.Open,
                CreatedAt = DateTime.UtcNow,

                Department = new Department
                {
                    Name = "TI"
                },
                CreatedBy = new User
                {
                    Name = "João"
                },
                AssignedAgent = null
            };


            _repositoryMock
                .Setup(r => r.GetByIdAsync(ticket.Id))
                .ReturnsAsync(ticket);

            var result = await _service.GetById(ticket.Id);

            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result.Id);
            Assert.Equal("TI", result.Department);
            Assert.Equal("João", result.CreatedBy);
            Assert.Null(result.AssignedAgent);
        }

        [Fact]
        public async Task GetById_ShouldReturnException_WhenTicketNotFound()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Ticket?)null);

            var exception = await Assert.ThrowsAsync<Exception>(
                () => _service.GetById(99)
            );

            Assert.Equal("Ticket não encontrado", exception.Message);
        }

        [Fact]
        public async Task GetConfirmationTicketByStatus_ShouldReturnBool()
        {
            int ticketId = 12;
            TicketStatusEnum ticketStatus = TicketStatusEnum.Seen;

            _repositoryMock
                .Setup(r => r.ExistsByStatusAsync(ticketId, ticketStatus))
                .ReturnsAsync(true);

            var result = await _service.GetConfirmationTicketByStatus(ticketId, ticketStatus);

            Assert.NotNull(result);
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task GetConfirmationTicketByUser_ShouldReturnTrue_WhenUserRelationIsCreated()
        {
            int userId = 12;
            int ticketId = 15;
            TicketUserRelation userRelation = TicketUserRelation.CreatedBy;

            _repositoryMock
                .Setup(r => r.ExistsCreatedByUserAsync(ticketId, userId))
                .ReturnsAsync(true);

            var result = await _service.GetConfirmationTicketByUser(userId, ticketId, userRelation);

            Assert.NotNull(result);
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task GetConfirmationTicketByUser_ShouldReturnTrue_WhenUserRelationIsAssignAgent()
        {
            int userId = 12;
            int ticketId = 15;
            TicketUserRelation userRelation = TicketUserRelation.AssignedTo;

            _repositoryMock
                .Setup(r => r.ExistsAssignedToUserAsync(ticketId, userId))
                .ReturnsAsync(true);

            var result = await _service.GetConfirmationTicketByUser(userId, ticketId, userRelation);

            Assert.NotNull(result);
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task GetDepartmentIdByTicket_ShouldReturnDepartmentId()
        {
            int ticketId = 18;
            int deparmentId = 1;

            _repositoryMock
                .Setup(r => r.GetDepartmentIdByTicketAsync(ticketId))
                .ReturnsAsync(deparmentId);

            var result = await _service.GetDepartmentIdByTicket(ticketId);

            Assert.NotNull(result);
            Assert.Equal(deparmentId, result);
        }

        [Fact]
        public async Task GetTicketAssignedByUser_ShouldReturnMappedTickets()
        {
            int agentId = 1;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    Title = "Erro no sistema",
                    DepartmentId = 1,
                    AssignedAgentId = agentId,
                    Status = TicketStatusEnum.Open
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Problema de login",
                    DepartmentId = 2,
                    AssignedAgentId = agentId,
                    Status = TicketStatusEnum.Seen
                }
            };

            _repositoryMock
                .Setup(r => r.GetAssignedToUserAsync(agentId))
                .ReturnsAsync(tickets);

            var result = await _service.GetTicketAssignedByUser(agentId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.All(result, dto =>
            {
                Assert.IsType<ResponseTicketDto>(dto);
            });

            Assert.Equal(tickets[0].Id, result[0].Id);
            Assert.Equal(tickets[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetTicketByStatus_ShouldReturnMappedTickets()
        {
            int userId = 1;
            TicketStatusEnum status = TicketStatusEnum.Open;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    Title = "Erro no sistema",
                    DepartmentId = 1,
                    CreatedById = userId,
                    Status = status
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Problema de login",
                    DepartmentId = 2,
                    CreatedById = userId,
                    Status = status
                }
            };

            _repositoryMock
                .Setup(r => r.GetByStatusAsync(status, userId))
                .ReturnsAsync(tickets);

            var result = await _service.GetTicketByStatus(status, userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.All(result, dto =>
            {
                Assert.IsType<ResponseTicketDto>(dto);
            });

            Assert.Equal(tickets[0].Id, result[0].Id);
            Assert.Equal(tickets[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetTicketByUser_ShouldReturnGroupedTickets()
        {
            var userId = 1;
            var departmentId = 2;

            var assignedTickets = new List<Ticket>
            {
                new Ticket { Id = 1, Title = "Chamado A" }
            };

            var createdTickets = new List<Ticket>
            {
                new Ticket { Id = 2, Title = "Chamado B" }
            };

            var departmentTickets = new List<Ticket>
            {
                new Ticket { Id = 3, Title = "Chamado C" }
            };

            _repositoryMock
                .Setup(r => r.GetAssignedToUserAsync(userId))
                .ReturnsAsync(assignedTickets);

            _repositoryMock
                .Setup(r => r.GetCreatedByUserAsync(userId))
                .ReturnsAsync(createdTickets);

            _repositoryMock
                .Setup(r => r.GetByDepartmentIdAsync(departmentId))
                .ReturnsAsync(departmentTickets);

            var result = await _service.GetTicketByUser(userId, departmentId);

            Assert.NotNull(result);

            Assert.Single(result.AssignedToMe);
            Assert.Single(result.CreatedByMe);
            Assert.Single(result.FromToMyDepartment);

            Assert.Equal(1, result.AssignedToMe[0].Id);
            Assert.Equal(2, result.CreatedByMe[0].Id);
            Assert.Equal(3, result.FromToMyDepartment[0].Id);
        }

        [Fact]
        public async Task GetTicketCreatedByUser_ShouldReturnMappedTickets()
        {
            int userId = 1;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    Title = "Erro no sistema",
                    DepartmentId = 1,
                    CreatedById = userId,
                    Status = TicketStatusEnum.Open
                },
                new Ticket
                {
                    Id = 2,
                    Title = "Problema de login",
                    DepartmentId = 2,
                    CreatedById = userId,
                    Status = TicketStatusEnum.Seen
                }
            };

            _repositoryMock
                .Setup(r => r.GetAssignedToUserAsync(userId))
                .ReturnsAsync(tickets);

            var result = await _service.GetTicketAssignedByUser(userId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.All(result, dto =>
            {
                Assert.IsType<ResponseTicketDto>(dto);
            });

            Assert.Equal(tickets[0].Id, result[0].Id);
            Assert.Equal(tickets[1].Id, result[1].Id);
        }

        [Fact]
        public async Task GetTicketPaged_ShouldReturnPagedReponse()
        {
            int page = 1;
            int pageSize = 10;

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    Title = "Erro no sistema",
                    DepartmentId = 1,
                    CreatedById = 2,
                    Status = TicketStatusEnum.Open
                }
            };

            _repositoryMock
                .Setup(c => c.CountAsync())
                .ReturnsAsync(tickets.Count);

            _repositoryMock
                 .Setup(p => p.GetPagedAsync(page, pageSize))
                 .ReturnsAsync(tickets);

            var result = await _service.GetTicketPaged(page, pageSize);

            Assert.NotNull(result);

            Assert.Equal(page, result.Page);
            Assert.Equal(tickets.Count, result.TotalItems);
            Assert.Equal(pageSize, result.PageSize);
        }

        [Fact]
        public async Task PatchStatus_ShouldReturnTrue_WhenUpdatedIsSucess()
        {
            int ticketId = 10;
            TicketStatusEnum status = TicketStatusEnum.Open;

            var ticket = new Ticket { Id = ticketId, Status = status, UpdatedAt =  DateTime.UtcNow };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            var result = await _service.PatchStatus(status, ticketId);

            Assert.NotNull(result);
            Assert.Equal(true, result);
        }

        [Fact]
        public async Task PatchStatus_ShouldReturnFalse_WhenTicketDoesNotExist()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Ticket?)null);

            var result = await _service.PatchStatus(TicketStatusEnum.Open, 99);

            Assert.NotNull(result);
            Assert.Equal(false, result);
        }

        [Fact]
        public async Task TransferAssingTicket_ShouldReturnMessage_WhenSucess()
        {
            int ticketId = 20;
            int newAgentId = 2;

            var ticket = new Ticket { Id = ticketId, AssignedAgentId = newAgentId, UpdatedAt = DateTime.UtcNow };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            var result = await _service.TransferAssingTicket(ticketId, newAgentId);

            Assert.NotNull(result);
            Assert.Equal("Chamado foi transferido com sucesso.", result);
        }

        [Fact]
        public async Task TransferAssingTicket_ShouldReturnMessage_WhenTicketDoesNotExist()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Ticket?)null);

            var result = await _service.TransferAssingTicket(99, 12);

            Assert.NotNull(result);
            Assert.Equal("Chamado não encontrado.", result);
        }
        [Fact]
        public async Task TransferAssingTicket_ShouldReturnMessage_WhenTicketIsAssigned()
        {
            int ticketId = 20;
            int newAgentId = 2;

            var ticket = new Ticket { Id = ticketId, AssignedAgentId = null, UpdatedAt = DateTime.UtcNow };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(ticketId))
                .ReturnsAsync(ticket);

            var result = await _service.TransferAssingTicket(ticketId, newAgentId);

            Assert.NotNull(result);
            Assert.Equal("Chamado não foi atribuida ainda.", result);
        }
    }
}
