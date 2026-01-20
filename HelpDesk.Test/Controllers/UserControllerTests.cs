using HelpDeskApi.Controllers;
using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
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
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IJwtService> _jwtServiceMock;


        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _authServiceMock = new Mock<IAuthService>();
            _jwtServiceMock = new Mock<IJwtService>();


            _controller = new UserController(_userServiceMock.Object, _authServiceMock.Object, _jwtServiceMock.Object);
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
        public async Task GetAll_ShouldReturn200OK_AndListUsers()
        {
            var users = new List<ResponseUserDto>
            {
                new ResponseUserDto
                {

                    Id = 1,
                    Name = "Gustavo",
                    Email = "Gustavo@gmail.com",
                    Department = "TI",
                    CreatedAt = DateTime.Now,
                }
            };

            _userServiceMock
                .Setup(r => r.GetAll())
                .ReturnsAsync(users);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<List<ResponseUserDto>>(okResult.Value);

            Assert.Single(value);
        }

        [Fact]
        public async Task GetById_ShouldReturn200OK_WhenUserExists()
        {
            var user = new ResponseUserDto
            {
                Id = 1,
                Name = "Milena",
                Email = "Milena@gmail.com",
                Department = "RH",
                CreatedAt = DateTime.Now,
            };

            _userServiceMock
                .Setup (r => r.GetById(user.Id))
                .ReturnsAsync(user);

            var result = await _controller.GetId(user.Id);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType< ResponseUserDto> (okResult.Value);

            Assert.Equal("Milena", value.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturn404NotFound_WhenUserDoesNotExist()
        {
            _userServiceMock
                .Setup(r => r.GetById(99))
                .ThrowsAsync(new Exception("Usuario não encontrado."));

            await Assert.ThrowsAsync<Exception>(() => _controller.GetId(99));
        }


        [Fact]
        public async Task GetUser_ShouldReturnOk_WhenUserExists()
        {
            SetUserClaims("1");

            var user = new ResponseUserDto 
            {
                Id = 1,
                Name = "Lucas",
                Email = "Lucas@gmail.com",
                Department = "Marketing",
                CreatedAt = DateTime.Now,
            };

            _userServiceMock
                .Setup(r => r.GetById(1))
                .ReturnsAsync(user);

            var result = await _controller.GetUser();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(user, okResult.Value);
        }

        [Fact]
        public async Task GetUser_ShouldReturnUnauthorized_WhenUserIdIsInvalid()
        {
            SetUserClaims("abc");

            var result = await _controller.GetUser();

            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            SetUserClaims("1");

            _userServiceMock
                .Setup(r => r.GetById(1))
                .ReturnsAsync((ResponseUserDto?)null);

            var result = await _controller.GetUser();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("{ mensagem = Usuario não encontrado. }", notFound.Value.ToString());
        }

        [Fact]
        public async Task CreatedUser_ShouldReturn201Created()
        {
            var dto = new CreateUserDto
            {
                Name = "Micael",
                Email = "Micael@gmail.com",
                Password = "123456",
                DepartmentId = 1
            };

            var user = new User
            {
                Id = 10,
                Name = "Gustavo",
                Email = dto.Email,
                DepartmentId = dto.DepartmentId
            };

            _userServiceMock
                .Setup(r => r.CreatedUser(dto))
                .ReturnsAsync(user);

            var result = await _controller.CreatedUser(dto);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var value = Assert.IsType<User>(createdResult.Value);

            Assert.Equal(nameof(_controller.GetId), createdResult.ActionName);

            Assert.NotNull(createdResult.RouteValues);
            Assert.Equal(10, createdResult.RouteValues["id"]);

            Assert.Equal(user, createdResult.Value);
        }

        [Fact]
        public async Task CreatedUser_ShouldReturnUnauthorized_WhenEmailIsInvalid()
        {
            var dto = new CreateUserDto
            {
                Email = "email_invalido",
                Name = "Gustavo",
                Password = "123456",
                DepartmentId = 1
            };

            var result = await _controller.CreatedUser(dto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);

            Assert.Contains("{ message = Email invalido. }", unauthorized.Value.ToString());

            _userServiceMock.Verify(
                s => s.CreatedUser(It.IsAny<CreateUserDto>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_ShouldReturn200Ok_AndToken()
        {
            var loginUser = new LoginUserDto
            {
                Email = "Jurandir@gmail.com",
                Password = "123456"
            };

            var user = new User
            {
                Id = 1,
                Email = loginUser.Email,
                Name = "Jurandir"
            };

            _authServiceMock
                .Setup(a => a.Login(loginUser))
                .ReturnsAsync(user);

            _jwtServiceMock
                .Setup(s => s.GenerateToken(user))
                .Returns("fake-jwt-token");

            var result = await _controller.Login(loginUser);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("{ token = fake-jwt-token }", okResult.Value.ToString());
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenEmailIsInvalid()
        {
            var loginUser = new LoginUserDto
            {
                Email = "Invalid Email",
                Password = "123456"
            };

            var result = await _controller.Login(loginUser);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("Email invalido", unauthorized.Value.ToString());

            _authServiceMock.Verify(
                a => a.Login(It.IsAny<LoginUserDto>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            var loginUser = new LoginUserDto
            {
                Email = "cleiton@gmail.com",
                Password = "Password Erro"
            };

            _authServiceMock
                .Setup(a => a.Login(loginUser))
                .ReturnsAsync((User?)null);

            var result = await _controller.Login(loginUser);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("Email ou senha inválidos", unauthorized.Value.ToString());
        }
    }
}
