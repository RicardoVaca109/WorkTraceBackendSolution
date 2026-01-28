using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkTrace.Api.Controllers;
using WorkTrace.Application.DTOs.AssignmentDTO.Management;
using WorkTrace.Application.Services;
using Xunit;

namespace WorkTrace.Tests.Controllers
{
    public class AssignmentControllerTests
    {
        private readonly Mock<IAssignmentService> _assignmentServiceMock;
        private readonly AssignmentController _controller;

        public AssignmentControllerTests()
        {
            _assignmentServiceMock = new Mock<IAssignmentService>();
            _controller = new AssignmentController(_assignmentServiceMock.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var request = new CreateAssignmentRequest();
            var response = new AssignmentResponse { Id = "123" };
            _assignmentServiceMock.Setup(s => s.CreateAssignmentAdminAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithList()
        {
            // Arrange
            var list = new List<AssignmentResponse> { new AssignmentResponse() };
            _assignmentServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(list);
        }

        [Fact]
        public async Task GetById_ShouldReturnResponse()
        {
            // Arrange
            var id = "123";
            var response = new AssignmentResponse { Id = id };
            _assignmentServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().Be(response);
        }

        [Fact]
        public async Task UpdateAssignment_ShouldReturnOk()
        {
            // Arrange
            var id = "123";
            var request = new UpdateAssignmentWebRequest();
            var response = new AssignmentResponse { Id = id };
            _assignmentServiceMock.Setup(s => s.UpdateAssignmentAsync(id, request)).ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateAssignment(id, request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetAssignmentTracking_ShouldReturnOk_WhenFound()
        {
            // Arrange
            var id = "123";
            var response = new AssignmentTrackingResponse { Id = id };
            _assignmentServiceMock.Setup(s => s.GetAssignmentTrackingAsync(id)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetAssignmentTracking(id);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetAssignmentTracking_ShouldReturnNotFound_WhenNull()
        {
            // Arrange
            var id = "123";
            _assignmentServiceMock.Setup(s => s.GetAssignmentTrackingAsync(id)).ReturnsAsync((AssignmentTrackingResponse?)null);

            // Act
            var result = await _controller.GetAssignmentTracking(id);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
}
