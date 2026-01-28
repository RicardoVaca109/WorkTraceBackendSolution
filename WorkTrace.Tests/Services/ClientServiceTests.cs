using AutoMapper;
using FluentAssertions;
using Moq;
using WorkTrace.Application.DTOs.ClientDTO.Information;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;
using WorkTrace.Logic.Services;
using Xunit;
using MongoDB.Bson;

namespace WorkTrace.Tests.Services
{
    public class ClientServiceTests
    {
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ClientService _clientService;

        public ClientServiceTests()
        {
            _clientRepositoryMock = new Mock<IClientRepository>();
            _mapperMock = new Mock<IMapper>();
            _clientService = new ClientService(_clientRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnClients()
        {
            // Arrange
            var clients = new List<Client> { new Client() };
            var dtos = new List<ClientInformationResponse> { new ClientInformationResponse() };
            _clientRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(clients);
            _mapperMock.Setup(m => m.Map<List<ClientInformationResponse>>(clients)).Returns(dtos);

            // Act
            var result = await _clientService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnClient_WhenFound()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var client = new Client { Id = id };
            var response = new ClientInformationResponse();

            _clientRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(client);
            _mapperMock.Setup(m => m.Map<ClientInformationResponse>(client)).Returns(response);

            // Act
            var result = await _clientService.GetByIdAsync(id);

            // Assert
            result.Should().Be(response);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            _clientRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync((Client?)null);

            // Act
            Func<Task> act = async () => await _clientService.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("ClienteS no encontrado.");
        }

        [Fact]
        public async Task CreateClientAsync_ShouldThrow_WhenDocumentExists()
        {
            // Arrange
            var request = new CreateClientRequest { DocumentNumber = "123" };
            _clientRepositoryMock.Setup(r => r.GetByDocumentNumberAsync("123")).ReturnsAsync(new Client());

            // Act
            Func<Task> act = async () => await _clientService.CreateClientAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Ya existe un cliente con este número de documento.");
        }

        [Fact]
        public async Task CreateClientAsync_ShouldCreate_WhenValid()
        {
            // Arrange
            var request = new CreateClientRequest { DocumentNumber = "123" };
            var client = new Client();
            var response = new ClientInformationResponse();

            _clientRepositoryMock.Setup(r => r.GetByDocumentNumberAsync("123")).ReturnsAsync((Client?)null);
            _mapperMock.Setup(m => m.Map<Client>(request)).Returns(client);
            _mapperMock.Setup(m => m.Map<ClientInformationResponse>(client)).Returns(response);

            // Act
            var result = await _clientService.CreateClientAsync(request);

            // Assert
            _clientRepositoryMock.Verify(r => r.CreateAsync(client), Times.Once);
            result.Should().Be(response);
        }

        [Fact]
        public async Task UpdateClientAsync_ShouldUpdateFields()
        {
             // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var request = new UpdateClientRequest { FullName = "New Name" };
            var client = new Client { Id = id, FullName = "Old Name" };
            var response = new ClientInformationResponse { FullName = "New Name" };

            _clientRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(client);
            _mapperMock.Setup(m => m.Map<ClientInformationResponse>(client)).Returns(response);

            // Act
            var result = await _clientService.UpdateClientAsync(id, request);

            // Assert
            client.FullName.Should().Be("New Name");
            _clientRepositoryMock.Verify(r => r.UpdateAsync(id, client), Times.Once);
        }
    }
}
