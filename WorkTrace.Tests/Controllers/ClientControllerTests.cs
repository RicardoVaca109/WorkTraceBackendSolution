using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WorkTrace.Api.Controllers;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.Services;
using Xunit;

namespace WorkTrace.Tests.Controllers
{
    public class ClientControllerTests
    {
        private readonly Mock<IClientService> _clientServiceMock;
        private readonly ClientController _controller;

        public ClientControllerTests()
        {
            _clientServiceMock = new Mock<IClientService>();
            _controller = new ClientController(_clientServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var list = new List<ClientInformationResponse>();
            _clientServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(list);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(list);
        }

        [Fact]
        public async Task GetById_ShouldReturnClient()
        {
            // Arrange
            var id = "1";
            var response = new ClientInformationResponse();
            _clientServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(response);

            // Act
            var result = await _controller.GetById(id);

            // Assert
            result.Should().Be(response);
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedClient()
        {
            // Arrange
            var request = new CreateClientRequest();
            var response = new ClientInformationResponse();
            _clientServiceMock.Setup(s => s.CreateClientAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Create(request);

            // Assert
            result.Should().Be(response);
        }

        [Fact]
        public async Task Update_ShouldReturnOk()
        {
            // Arrange
            var id = "1";
            var request = new UpdateClientRequest();
            var response = new ClientInformationResponse();
            _clientServiceMock.Setup(s => s.UpdateClientAsync(id, request)).ReturnsAsync(response);

            // Act
            var result = await _controller.Update(id, request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be(response);
        }
    }
}
