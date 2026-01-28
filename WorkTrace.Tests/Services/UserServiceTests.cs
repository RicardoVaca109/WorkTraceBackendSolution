using AutoMapper;
using FluentAssertions;
using Moq;
using WorkTrace.Application.DTOs.UserDTO.Information;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;
using WorkTrace.Logic.Services;
using Xunit;
using MongoDB.Bson;

namespace WorkTrace.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _mapperMock = new Mock<IMapper>();
            _userService = new UserService(_userRepositoryMock.Object, _jwtServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnUsers()
        {
            // Arrange
            var users = new List<User> { new User() };
            var dtos = new List<UserInformationResponse> { new UserInformationResponse() };
            _userRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(users);
            _mapperMock.Setup(m => m.Map<List<UserInformationResponse>>(users)).Returns(dtos);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(dtos);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDocumentExists()
        {
            // Arrange
            var request = new CreateUserRequest { DocumentNumber = "123" };
            _userRepositoryMock.Setup(r => r.GetByDocumentNumberAsync("123")).ReturnsAsync(new User());

            // Act
            Func<Task> act = async () => await _userService.CreateAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Ya existe un usuario con este número de documento");
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateUser_WhenValid()
        {
            // Arrange
            var request = new CreateUserRequest { DocumentNumber = "123", Email = "test@test.com", Password = "pass" };
            var user = new User();
            var response = new UserInformationResponse();

            _userRepositoryMock.Setup(r => r.GetByDocumentNumberAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            _mapperMock.Setup(m => m.Map<User>(request)).Returns(user);
            _mapperMock.Setup(m => m.Map<UserInformationResponse>(user)).Returns(response);

            // Act
            var result = await _userService.CreateAsync(request);

            // Assert
            _userRepositoryMock.Verify(r => r.CreateAsync(user), Times.Once);
            user.Password.Should().NotBe("pass"); // Should be hashed
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            // Arrange
            var email = "test@test.com";
            var password = "password";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Email = email, Password = hashedPassword };
            
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(user);
            _jwtServiceMock.Setup(s => s.GenerateToken(user)).Returns("token_abc");

            // Act
            var result = await _userService.LoginAsync(email, password);

            // Assert
            result.Token.Should().Be("token_abc");
            result.ExpireAt.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () => await _userService.LoginAsync("a", "b");

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Credenciales Incorrectas");
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateFields()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var request = new UpdateUserRequest { FullName = "New Name" };
            var user = new User { Id = id, FullName = "Old Name" };
            var response = new UserInformationResponse { FullName = "New Name" };

            _userRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(user);
            _mapperMock.Setup(m => m.Map<UserInformationResponse>(user)).Returns(response);

            // Act
            var result = await _userService.UpdateAsync(id, request);

            // Assert
            user.FullName.Should().Be("New Name");
            _userRepositoryMock.Verify(r => r.UpdateAsync(id, user), Times.Once);
        }

        [Fact]
        public async Task SetInactiveUser_ShouldSetFalse()
        {
             // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var user = new User { IsActive = true };
            _userRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(user);

            // Act
            var result = await _userService.SetInactiveUser(id);

            // Assert
            result.Should().BeTrue();
            user.IsActive.Should().BeFalse();
            _userRepositoryMock.Verify(r => r.UpdateAsync(id, user), Times.Once);
        }
    }
}
