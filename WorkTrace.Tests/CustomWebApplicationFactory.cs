using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WorkTrace.Application.Repositories;

namespace WorkTrace.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IUserRepository> UserRepositoryMock { get; } = new();
    public Mock<IAssignmentRepository> AssignmentRepositoryMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing IUserRepository
            var userDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IUserRepository));
            if (userDescriptor != null) services.Remove(userDescriptor);

            // Remove existing IAssignmentRepository
            var assignmentDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAssignmentRepository));
            if (assignmentDescriptor != null) services.Remove(assignmentDescriptor);

            // Add mocks
            services.AddSingleton(UserRepositoryMock.Object);
            services.AddSingleton(AssignmentRepositoryMock.Object);
        });
    }
}
