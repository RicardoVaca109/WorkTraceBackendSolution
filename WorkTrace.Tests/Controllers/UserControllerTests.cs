using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkTrace.Api.Controllers;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Services;
using Xunit;

namespace WorkTrace.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _controller = new UserController(_userServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var list = new List<UserInformationResponse>();
            _userServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(list);
        }

        [Fact]
        public async Task GetById_ShouldReturnUser()
        {
            // Arrange
            var id = "1";
            var user = new UserInformationResponse();
            _userServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().Be(user);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedUser()
        {
            // Arrange
            var request = new CreateUserRequest();
            var response = new UserInformationResponse();
            _userServiceMock.Setup(s => s.CreateAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().Be(response);
        }

        [Fact]
        public async Task Login_ShouldReturnOk()
        {
            // Arrange
            var request = new LoginRequest { Email = "a", Password = "b" };
            var response = new LoginResponse();
            _userServiceMock.Setup(s => s.LoginAsync(request.Email, request.Password)).ReturnsAsync(response);

            // Act
            var result = await _controller.Login(request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }

        [Fact]
        public async Task DeactivateUser_ShouldReturnOk_WhenSuccess()
        {
            // Arrange
            var id = "1";
            _userServiceMock.Setup(s => s.SetInactiveUser(id)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeactivateUser(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeactivateUser_ShouldReturnNotFound_WhenFail()
        {
            // Arrange
            var id = "1";
            _userServiceMock.Setup(s => s.SetInactiveUser(id)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeactivateUser(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
