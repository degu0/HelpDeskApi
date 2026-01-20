using HelpDeskApi.DTOs;
using HelpDeskApi.Model;
using HelpDeskApi.Repositories.Interfaces;
using HelpDeskApi.Service;
using HelpDeskApi.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace HelpDeskApi.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordService> _passwordServiceMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();

            _service = new UserService(
                _userRepositoryMock.Object,
                _passwordServiceMock.Object
            );
        }

        [Fact]
        public async Task CreatedUser_ShouldCryptThePassword_AndSalveUser()
        {
            var dto = new CreateUserDto
            {
                Name = "Gustavo",
                Email = "Gustavo@gmail.com",
                Password = "123456",
                DepartmentId = 1,
            };

            _passwordServiceMock
                .Setup(p => p.HashPassword("123456"))
                .Returns("senha_hash");

            _userRepositoryMock
                .Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);


            var result = await _service.CreatedUser(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.DepartmentId, result.DepartmentId);
            Assert.Equal("senha_hash", result.PasswordHash);

            _passwordServiceMock.Verify(
                p => p.HashPassword("123456"),
                Times.Once
            );

            _userRepositoryMock.Verify(
                r => r.CreateAsync(It.IsAny<User>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnListUsers()
        {
            var users = new List<User>
            {
                new User
                {

                    Id = 1,
                    Name = "Gustavo",
                    Email = "Gustavo@gmail.com",
                    PasswordHash = "123456",
                    Department = new Department
                    {
                        Id = 1,
                        Name = "TI"
                    },
                    CreatedAt = DateTime.Now,
                }
            };

            _userRepositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(users);

            var result = await _service.GetAll();

            Assert.NotNull(result);
            Assert.Single(result);

            Assert.Equal(users[0].Id, result[0].Id);
            Assert.Equal(users[0].Name, result[0].Name);
            Assert.Equal(users[0].Email, result[0].Email);

            _userRepositoryMock.Verify(
                r => r.GetAllAsync(),
                Times.Once
            );
        }

        [Fact]
        public async Task GetDepartmentByUser_ShouldReturnDepartmentId()
        {
            var userId = 1;
            var departmentId = 3;

            _userRepositoryMock
                .Setup(r => r.GetDepartmentIdAsync(userId))
                .ReturnsAsync(departmentId);

            var result = await _service.GetDepartmentByUser(1);
            Assert.Equal(departmentId, result);

            _userRepositoryMock.Verify(
                r => r.GetDepartmentIdAsync(userId),
                Times.Once
            );
        }

        [Fact]
        public async Task GetById_ShouldReturnDepartment_ThenExist()
        {
            var user = new User
            {
                Id = 1,
                Name = "Gustavo",
                Email = "Gustavo@gmail.com",
                PasswordHash = "123456",
                DepartmentId = 1,
                Department = new Department
                {
                    Id = 1,
                    Name = "TI"
                },
                CreatedAt = DateTime.Now,
            };

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            var result = await _service.GetById(1);

            Assert.Equal(user.Id, result.Id);
            Assert.Equal(user.Name, result.Name);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetById_GivenReturnException_ThenNotExist()
        {
            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.GetById(99)
            );
        }
    }
}
