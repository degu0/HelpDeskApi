using HelpDeskApi.Model;
using HelpDeskApi.Repositories.Interfaces;
using HelpDeskApi.Service;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDeskApi.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _repositoryMock;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _repositoryMock = new Mock<IDepartmentRepository>();
            _service = new DepartmentService(_repositoryMock.Object);
        }

        [Fact]
        public async Task CreateDepartment_ShouldReturnCreatedDepartment()
        {
            var department = new Department
            {
                Id = 1,
                Name = "Marketing"
            };

            _repositoryMock
                .Setup(r => r.CreateAsync(department))
                .ReturnsAsync(department);

            var result = await _service.CreateDepartment(department);

            Assert.NotNull(result);
            Assert.Equal(department.Id, result.Id);
            Assert.Equal(department.Name, result.Name);
        }

        [Fact]
        public async Task GetAll_ShouldReturnDeparmentList()
        {
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "TI"},
                new Department { Id = 2, Name = "Finance"}
            };

            _repositoryMock
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(departments);

            var result = await _service.GetAll();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetById_GivenReturnDepartment_ThenExist()
        {
            var department = new Department
            {
                Id = 1,
                Name = "Finance"
            };

            _repositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(department);

            var result = await _service.GetById(1);

            Assert.NotNull(result);
            Assert.Equal("Finance", result.Name);
        }

        [Fact]
        public async Task GetById_GivenReturnException_ThenNotExist()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Department?) null);

            await Assert.ThrowsAsync<Exception>(() => _service.GetById(99));
        }
    }
}
