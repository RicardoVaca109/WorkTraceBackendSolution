using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Moq;
using WorkTrace.Application.DTOs.UserDTO.Login;
using WorkTrace.Data.Models;
using Xunit;

namespace WorkTrace.Tests;

public class WorkTraceApiIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public WorkTraceApiIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task FullSecurityLifecycle_Test()
    {
        // 1. Verify 401 Unauthorized for /api/Assignment/GetAll (Security Check)
        // The controller route is [Route("[controller]/[action]")], so it's /Assignment/GetAll
        var unauthorizedResponse = await _client.GetAsync("/Assignment/GetAll");
        unauthorizedResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 2. Perform a POST to Login to retrieve a real JWT (mocking user success)
        var email = "test@example.com";
        var password = "Password123!";
        // We use BCrypt to hash the password because the real UserService verifies it against the repo user
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Id = "user_id_123",
            Email = email,
            Password = hashedPassword,
            IsActive = true,
            Role = Application.Enums.UserRoles.Administrador, // Assuming Admin has access
            FullName = "Test User",
            DocumentNumber = "1234567890",
            PhoneNumber = "555-5555"
        };

        // Setup the mock to return this user when looked up by email
        _factory.UserRepositoryMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        var loginRequest = new LoginRequest
        {
            Email = email,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync("/User/Login", loginRequest);

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
        loginResult.Should().NotBeNull();
        loginResult!.Token.Should().NotBeNullOrEmpty();

        // 3. Use that JWT to successfully access protected endpoints (200 OK)
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult.Token);

        // Setup Assignment Mock for GetAll
        // Assuming the service calls GetAsync() on the repository
        _factory.AssignmentRepositoryMock.Setup(x => x.GetAsync())
            .ReturnsAsync(new List<Assignment>
            {
                new Assignment { Id = "assignment_1", Address = "123 Test St" }
            });

        var authorizedResponse = await _client.GetAsync("/Assignment/GetAll");
        authorizedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var assignments = await authorizedResponse.Content.ReadFromJsonAsync<List<Application.DTOs.AssignmentDTO.Management.AssignmentResponse>>();
        assignments.Should().NotBeNull();
    }
}
