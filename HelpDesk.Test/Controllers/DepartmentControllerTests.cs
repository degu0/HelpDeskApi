using HelpDeskApi.Controllers;
using HelpDeskApi.Model;
using HelpDeskApi.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpDeskApi.Tests.Controllers
{
    public class DepartmentControllerTests
    {
        private readonly Mock<IDepartmentService> _serviceMock;
        private readonly DepartmentController _controller;

        public DepartmentControllerTests()
        {
            _serviceMock = new Mock<IDepartmentService>();
            _controller = new DepartmentController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturn200OK_WithDepartmentList()
        {
            var departments = new List<Department>
            {
                new Department { Id = 1, Name = "TI"},
                new Department { Id = 2, Name = "Marketing"}
            };

            _serviceMock
                .Setup(r => r.GetAll())
                .ReturnsAsync(departments);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<List<Department>>(okResult.Value);

            Assert.Equal(2, value.Count);
        }

        [Fact]
        public async Task GetById_ShoulReturn200OK_WhenExitDepartment()
        {
            var department = new Department { Id = 1, Name = "RH" };

            _serviceMock
                .Setup(r => r.GetById(1))
                .ReturnsAsync(department);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var value = Assert.IsType<Department>(okResult.Value);
            
            Assert.Equal("RH", value.Name);
        }

        [Fact]
        public async Task GetById_ShouldReturn404NotFound_WhenDepartmentDoesNotExist()
        {
            _serviceMock
                .Setup(r => r.GetById(99))
                .ThrowsAsync(new Exception("Department não encontrado."));

            await Assert.ThrowsAsync<Exception>(() => _controller.GetById(99));
        }

        [Fact]
        public async Task CreateDepartment_ShouldReturn201Created()
        {
            var department = new Department { Id = 1, Name = "Finance" };

            _serviceMock
                .Setup(r => r.CreateDepartment(department))
                .ReturnsAsync(department);

            var result = await _controller.CreateDepartment(department);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);

            var value = Assert.IsType<Department>(createdResult.Value);

            Assert.Equal("Finance", value.Name);
        }
    }
}
