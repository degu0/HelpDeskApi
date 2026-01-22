using HelpDeskApi.Controllers;
using HelpDeskApi.Domain.Enum;
using HelpDeskApi.DTOs;
using HelpDeskApi.Models;
using HelpDeskApi.Service;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace HelpDeskApi.Tests.Controllers
{
    public class TicketControllerTests
    {
        private readonly Mock<ITicketService> _ticketServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly TicketController _controller;

        public TicketControllerTests()
        {
            _ticketServiceMock = new Mock<ITicketService>();
            _userServiceMock = new Mock<IUserService>();
            _controller = new TicketController(_ticketServiceMock.Object, _userServiceMock.Object);
        }

        private void SetUserClaims(string userId)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        };

            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = claimsPrincipal
                }
            };
        }

        [Fact]
        public async Task GetTickets_ShouldReturn200Ok_AndListTickets()
        {
            var tickets = new PagedResponse<ResponseTicketPocketDto>
            {
                Page = 1,
                PageSize = 10,
                TotalItems = 1,
                TotalPages = 1,
                Items = new List<ResponseTicketPocketDto>
                {
                    new ResponseTicketPocketDto
                    {
                        Id = 1,
                        Title = "Title",
                        Status = "Open",
                        CreatedAt = DateTime.UtcNow,
                    }
                }
            };

            _ticketServiceMock
                .Setup(r => r.GetTicketPaged(tickets.Page, tickets.PageSize))
                .ReturnsAsync(tickets);

            var result = await _controller.GetTickets();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<PagedResponse<ResponseTicketPocketDto>>(okResult.Value);

            Assert.NotNull(result);
            Assert.Equal(tickets.Page, value.Page);
            Assert.Equal(tickets.Items.Count, value.Items.Count);
        }

        [Fact]
        public async Task GetTickets_ShouldReturnBaRequest_WhenPageOrPageSizeIsNull()
        {
            int page = -1;
            int pageSize = -1;

            var result = await _controller.GetTickets(page, pageSize);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetById_ShouldReturn200Ok_AndTicket()
        {
            var ticket = new ResponseTicketDto
            {
                Id = 1,
                Title = "Title test",
                Description = "Description test",
                Status = "Seen",
                Department = "TI",
                CreatedBy = "Gustavo",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _ticketServiceMock
                .Setup(r => r.GetById(ticket.Id))
                .ReturnsAsync(ticket);

            var result = await _controller.GetById(ticket.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<ResponseTicketDto>(okResult.Value);

            Assert.Equal(ticket.Id, value.Id);
            Assert.Equal(ticket.Title, value.Title);
            Assert.Equal(ticket.Description, value.Description);
        }

        [Fact]
        public async Task GetById_ShoulReturn404NotFound_WhenTicketDoesNotExist()
        {
            _ticketServiceMock
                .Setup(r => r.GetById(99))
                .ReturnsAsync((ResponseTicketDto?)null);

            var result = await _controller.GetById(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("{ mensagem = Chamado não encontrado. }", notFound.Value.ToString());
        }

        [Fact]
        public async Task CreateTicket_ShouldReturn201Created()
        {
            SetUserClaims("1");

            var dto = new CreateTicketDto
            {
                Title = "Title test",
                Description = "Description test",
                DepartmentId = 1
            };

            var ticket = new Ticket
            {
                Id = 10,
                Title = dto.Title,
                Description = dto.Description,
                DepartmentId = dto.DepartmentId
            };

            _ticketServiceMock
                .Setup(r => r.CreatTicket(dto, 1))
                .ReturnsAsync(ticket);

            var result = await _controller.CreateTicket(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var value = Assert.IsType<Ticket>(createdResult.Value);

            Assert.Equal(ticket.Title, value.Title);
        }

        [Fact]
        public async Task CreateTicket_ShouldReturnUnauthorized_WhenUserIdInvalid()
        {
            SetUserClaims("abc");

            var dto = new CreateTicketDto
            {
                Title = "Title test",
                Description = "Description test",
                DepartmentId = 1
            };

            var result = await _controller.CreateTicket(dto);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetByDeparment_ShouldReturn200Ok_AndListTickets() 
        {
            SetUserClaims("1");

            var tickets = new List<ResponseTicketDto>
            {
                new ResponseTicketDto
                {
                    Id = 1,
                    Title = "Title test",
                    Description = "Description test",
                    Status = "Seen",
                    Department = "TI",
                    CreatedBy = "Gustavo",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            _userServiceMock
                .Setup(u => u.GetDepartmentByUser(1))
                .ReturnsAsync(10);

            _ticketServiceMock
                .Setup(t => t.GetByDepartment(10))
                .ReturnsAsync(tickets);

            var result = await _controller.GetByDepartment();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<List<ResponseTicketDto>>(okResult.Value);

            Assert.Equal(tickets.Count, value.Count);
        }

        [Fact]
        public async Task GetByDeparment_ShouldReturnUnauthorized_WhenUserIdInvalid() 
        {
            SetUserClaims("abc");

            var result = await _controller.GetByDepartment();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task AssignTicket_ShouldReturn200Ok_AndMessage() 
        {
            SetUserClaims("2");

            int ticketId = 3;
            int departmentId = 4;

            _userServiceMock
                .Setup(u => u.GetDepartmentByUser(2))
                .ReturnsAsync(departmentId);

            _ticketServiceMock
                .Setup(t => t.GetDepartmentIdByTicket(ticketId))
                .ReturnsAsync(departmentId);

            _ticketServiceMock
                .Setup(t => t.AssignTicket(ticketId, 2))
                .ReturnsAsync("Chamado foi atribuido com sucesso.");

            var result = await _controller.AssignTicket(ticketId);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("{ message = Chamado foi atribuido com sucesso. }", okResult.Value.ToString());
        }

        [Fact]
        public async Task AssignTicket_ShouldReturnUnauthorized_WhenUserIdInvalid() 
        {
            SetUserClaims("abc");

            var result = await _controller.AssignTicket(1);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task AssignTicket_ShouldReturnUnauthorized_WhenUserDepartmentIsNotEqualForTicketDeparment() 
        {
            SetUserClaims("2");

            int ticketId = 3;

            _userServiceMock
                .Setup(u => u.GetDepartmentByUser(2))
                .ReturnsAsync(8);

            _ticketServiceMock
                .Setup(t => t.GetDepartmentIdByTicket(ticketId))
                .ReturnsAsync(5);

            var result = await _controller.AssignTicket(ticketId);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);

            Assert.Equal("{ message = O departamento do usuario não é o mesmo do chamado. }", unauthorized.Value.ToString());
        }

        [Fact]
        public async Task AssignTicket_ShouldReturnBadRequest_WhenTicketDoesNotExist() 
        {
            SetUserClaims("2");

            int ticketId = 3;
            int departmentId = 4;

            _userServiceMock
                .Setup(u => u.GetDepartmentByUser(2))
                .ReturnsAsync(departmentId);

            _ticketServiceMock
                .Setup(t => t.GetDepartmentIdByTicket(ticketId))
                .ReturnsAsync(departmentId);

            _ticketServiceMock
                .Setup(t => t.AssignTicket(ticketId, 2))
                .ReturnsAsync("Chamado não encontrado.");

            var result = await _controller.AssignTicket(ticketId);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("{ message = Chamado não encontrado. }", badRequest.Value.ToString());
        }

        [Fact]
        public async Task AssignTicket_ShouldReturnBadRequest_WhenTicketIsAssigned() 
        {
            SetUserClaims("2");

            int ticketId = 3;
            int departmentId = 4;

            _userServiceMock
                .Setup(u => u.GetDepartmentByUser(2))
                .ReturnsAsync(departmentId);

            _ticketServiceMock
                .Setup(t => t.GetDepartmentIdByTicket(ticketId))
                .ReturnsAsync(departmentId);

            _ticketServiceMock
                .Setup(t => t.AssignTicket(ticketId, 2))
                .ReturnsAsync("Chamado ja foi atribuido.");

            var result = await _controller.AssignTicket(ticketId);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);

            Assert.Equal("{ message = Chamado ja foi atribuido. }", badRequest.Value.ToString());
        }

        [Fact]
        public async Task GetTicketCreatedByUser_ShouldReturn200Ok_AndListTickets() 
        {
            SetUserClaims("3");

            var tickets = new List<ResponseTicketDto>
            {
                new ResponseTicketDto
                {
                    Id = 1,
                    Title = "Title test",
                    Description = "Description test",
                    Status = "Seen",
                    Department = "TI",
                    CreatedBy = "Gustavo",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            _ticketServiceMock
                .Setup(r => r.GetTicketCreatedByUser(3))
                .ReturnsAsync(tickets);

            var result = await _controller.GetTicketCreatedByUser();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<List<ResponseTicketDto>>(okResult.Value);

            Assert.Equal(1, value.Count);
        }

        [Fact]
        public async Task GetTicketCreatedByUser_ShouldReturnUnauthorized_WhenUserIdInvalid() 
        {
            SetUserClaims("abc");

            var result = await _controller.GetTicketCreatedByUser();

            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public async Task GetTicketCreatedByUser_ShouldReturn200Ok_WhenDoesNotTicketInTheBox() 
        {
            SetUserClaims("3");

            _ticketServiceMock
                .Setup(r => r.GetTicketCreatedByUser(3))
                .ReturnsAsync((List<ResponseTicketDto>?)null);

            var result = await _controller.GetTicketCreatedByUser();

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("{ message = A sua caixa de chamados esta vazia. }", okResult.Value.ToString());
        }

        [Fact]
        public async Task GetTicketAssignByUser_ShouldReturn200Ok_AndListTickets() 
        {
            SetUserClaims("4");

            var tickets = new List<ResponseTicketDto>
            {
                new ResponseTicketDto
                {
                    Id = 1,
                    Title = "Title test",
                    Description = "Description test",
                    Status = "Seen",
                    Department = "TI",
                    CreatedBy = "Gustavo",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            _ticketServiceMock
                .Setup(r => r.GetTicketAssignedByUser(4))
                .ReturnsAsync(tickets);

            var result = await _controller.GetTicketAssignByUser();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<List<ResponseTicketDto>>(okResult.Value);

            Assert.Equal(1, value.Count);
        }

        [Fact]
        public async Task GetTicketAssignByUser_ShouldReturnUnauthorized_WhenUserIdInvalid() 
        {
            SetUserClaims("abc");

            var result = await _controller.GetTicketAssignByUser();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetTicketAssignByUser_ShouldReturn200Ok_WhenDoesNotTicketInTheBox() 
        {
            SetUserClaims("4");

            _ticketServiceMock
                .Setup(r => r.GetTicketAssignedByUser(4))
                .ReturnsAsync((List<ResponseTicketDto>?)null);

            var result = await _controller.GetTicketAssignByUser();

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("{ message = A sua caixa de chamados esta vazia. }", okResult.Value.ToString());
        }

        [Fact]
        public async Task GetTicketByStatus_ShouldReturn200Ok_AndListTickets() 
        {
            SetUserClaims("5");

            var tickets = new List<ResponseTicketDto>
            {
                new ResponseTicketDto
                {
                    Id = 1,
                    Title = "Title test",
                    Description = "Description test",
                    Status = "Seen",
                    Department = "TI",
                    CreatedBy = "Gustavo",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };
            TicketStatusEnum status = TicketStatusEnum.Seen;

            _ticketServiceMock
                .Setup(r => r.GetTicketByStatus(status, 5))
                .ReturnsAsync(tickets);

            var result = await _controller.GetTicketByStatus(status);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<List<ResponseTicketDto>>(okResult.Value);

            Assert.Equal(1, value.Count);
        }

        [Fact]
        public async Task GetTicketByStatus_ShouldReturnUnauthorized_WhenUserIdInvalid() 
        {
            SetUserClaims("abc");

            var result = await _controller.GetTicketByStatus(TicketStatusEnum.Seen);

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetTicketByStatus_ShouldReturn200Ok_WhenDoesNotTicketInTheBox() 
        {
            SetUserClaims("4");

            TicketStatusEnum status = TicketStatusEnum.Seen;

            _ticketServiceMock
                .Setup(r => r.GetTicketByStatus(status, 4))
                .ReturnsAsync((List<ResponseTicketDto>?)null);

            var result = await _controller.GetTicketByStatus(status);

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal("{ message = A sua caixa de chamados esta vazia. }", okResult.Value.ToString());
        }

        [Fact]
        public async Task PatchStatus_ShouldReturn200Ok_AndMessage() { }

        [Fact]
        public async Task PatchStatus_ShouldReturnBadRequest_WhenStatusIsInvalid() { }

        [Fact]
        public async Task PatchStatus_ShouldReturnUnauthorized_WhenUserIdInvalid() { }

        [Fact]
        public async Task PatchStatus_ShouldReturn404NotFound_WhenTicketIsNotByUser() { }

        [Fact]
        public async Task PatchStatus_ShouldReturnBadRequestAndMessage_WhenItWasNotUpdated() { }

        [Fact]
        public async Task ClosedTicket_ShouldReturn200Ok_AndMessage() { }

        [Fact]
        public async Task ClosedTicket_ShouldReturnBadRequest_WhenStatusIsInvalid() { }

        [Fact]
        public async Task ClosedTicket_ShouldReturnUnauthorized_WhenUserIdInvalid() { }

        [Fact]
        public async Task ClosedTicket_ShouldReturn404NotFound_WhenTicketIsNotByUser() { }

        [Fact]
        public async Task ClosedTicket_ShouldReturnBadRequestAndMessage_WhenItWasNotUpdated() { }

        [Fact]
        public async Task ReopenTicket_ShouldReturn200Ok_AndMessage() { }

        [Fact]
        public async Task ReopenTicket_ShouldReturnBadRequest_WhenStatusIsInvalid() { }

        [Fact]
        public async Task ReopenTicket_ShouldReturnUnauthorized_WhenUserIdInvalid() { }

        [Fact]
        public async Task ReopenTicket_ShouldReturn404NotFound_WhenTicketIsNotByUser() { }

        [Fact]
        public async Task ReopenTicket_ShouldReturnBadRequestAndMessage_WhenItWasNotUpdated() { }

        [Fact]
        public async Task SoftDeleteTicket_ShouldReturn200Ok_AndMessage() { }

        [Fact]
        public async Task SoftDeleteTicket_ShouldReturnBadRequest_WhenStatusIsInvalid() { }

        [Fact]
        public async Task SoftDeleteTicket_ShouldReturnUnauthorized_WhenUserIdInvalid() { }

        [Fact]
        public async Task SoftDeleteTicket_ShouldReturn404NotFound_WhenTicketIsNotByUser() { }

        [Fact]
        public async Task SoftDeleteTicket_ShouldReturnBadRequestAndMessage_WhenItWasNotUpdated() { }

        [Fact]
        public async Task TransferTicket_ShouldReturn200Ok_AndMessage() { }

        [Fact]
        public async Task TransferTicket_ShouldReturnUnauthorized_WhenUserIdInvalid() { }

        [Fact]
        public async Task TransferTicket_ShouldReturnUnauthorized_WhenUserDepartmentIsNotEqualForTicketDeparment() { }

        [Fact]
        public async Task TransferTicket_ShouldReturnBadRequest_WhenTicketDoesNotExist() { }

        [Fact]
        public async Task TransferTicket_ShouldReturnBadRequest_WhenTicketIsAssigned() { }

        [Fact]
        public async Task GetAllTicketByUser_ShouldReturn200Ok_AndListTickets() { }

        [Fact]
        public async Task GetAllTicketByUser_ShouldReturnUnauthorized_WhenUserIdInvalid() { }
    }
}
