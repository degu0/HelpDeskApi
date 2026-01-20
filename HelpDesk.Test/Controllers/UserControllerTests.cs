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
    }
}
