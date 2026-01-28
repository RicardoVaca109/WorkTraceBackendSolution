using AutoMapper;
using FluentAssertions;
using Moq;
using WorkTrace.Application.DTOs.AssignmentDTO.Management;
using WorkTrace.Application.DTOs.AssignmentDTO.Mobile;
using WorkTrace.Application.DTOs.FormTemplateDTO.Information;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;
using WorkTrace.Logic.Services;
using Xunit;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace WorkTrace.Tests.Services
{
    public class AssignmentServiceTests
    {
        private readonly Mock<IAssignmentRepository> _assignmentRepositoryMock;
        private readonly Mock<IClientRepository> _clientRepositoryMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IFormTemplateRepository> _formTemplateRepositoryMock;
        private readonly Mock<IGeocodingService> _geocodingServiceMock;
        private readonly Mock<IInstallationStepRepository> _installationStepRepositoryMock;
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly Mock<IStatusRepository> _statusRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly AssignmentService _assignmentService;

        public AssignmentServiceTests()
        {
            _assignmentRepositoryMock = new Mock<IAssignmentRepository>();
            _clientRepositoryMock = new Mock<IClientRepository>();
            _fileServiceMock = new Mock<IFileService>();
            _formTemplateRepositoryMock = new Mock<IFormTemplateRepository>();
            _geocodingServiceMock = new Mock<IGeocodingService>();
            _installationStepRepositoryMock = new Mock<IInstallationStepRepository>();
            _serviceRepositoryMock = new Mock<IServiceRepository>();
            _statusRepositoryMock = new Mock<IStatusRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();

            _assignmentService = new AssignmentService(
                _assignmentRepositoryMock.Object,
                _clientRepositoryMock.Object,
                _fileServiceMock.Object,
                _formTemplateRepositoryMock.Object,
                _geocodingServiceMock.Object,
                _installationStepRepositoryMock.Object,
                _serviceRepositoryMock.Object,
                _statusRepositoryMock.Object,
                _userRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateAssignmentAdminAsync_ShouldCreateAssignment_WhenDataIsValid()
        {
            // Arrange
            var request = new CreateAssignmentRequest
            {
                Client = "client1",
                Service = "service1",
                Status = "status1",
                Users = new List<string> { "user1" },
                Address = "123 Main St"
            };

            var id = ObjectId.GenerateNewId().ToString();
            var assignment = new Assignment { Id = id };
            var assignmentResponse = new AssignmentResponse { Id = id };
            var geoPoint = new GeoPoint { Latitude = 10, Longitude = 20 };

            _clientRepositoryMock.Setup(r => r.GetAsync(request.Client)).ReturnsAsync(new Client());
            _serviceRepositoryMock.Setup(r => r.GetAsync(request.Service)).ReturnsAsync(new Service());
            _statusRepositoryMock.Setup(r => r.GetAsync(request.Status)).ReturnsAsync(new Status());
            _userRepositoryMock.Setup(r => r.GetAsync(It.IsAny<string>())).ReturnsAsync(new User());

            _mapperMock.Setup(m => m.Map<Assignment>(request)).Returns(assignment);
            _geocodingServiceMock.Setup(s => s.GetGeoPointAsync(request.Address)).ReturnsAsync(geoPoint);
            _mapperMock.Setup(m => m.Map<AssignmentResponse>(assignment)).Returns(assignmentResponse);

            // Act
            var result = await _assignmentService.CreateAssignmentAdminAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(assignment.Id.ToString());
            _assignmentRepositoryMock.Verify(r => r.CreateAsync(assignment), Times.Once);
        }

        [Fact]
        public async Task CreateAssignmentAdminAsync_ShouldThrowException_WhenClientDoesNotExist()
        {
            // Arrange
            var request = new CreateAssignmentRequest { Client = "nonexistent" };
            _clientRepositoryMock.Setup(r => r.GetAsync(request.Client)).ReturnsAsync((Client?)null);

            // Act
            Func<Task> act = async () => await _assignmentService.CreateAssignmentAdminAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Cliente no existe");
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAssignments_WithResolvedForms()
        {
            // Arrange
            var assignments = new List<Assignment>
            {
                new Assignment { Id = ObjectId.GenerateNewId().ToString(), AssignedForms = new List<string> { "form1" } }
            };
            var dtos = new List<AssignmentResponse>
            {
                new AssignmentResponse { Id = assignments[0].Id.ToString() }
            };

            _assignmentRepositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(assignments);
            _mapperMock.Setup(m => m.Map<List<AssignmentResponse>>(assignments)).Returns(dtos);

            _formTemplateRepositoryMock.Setup(r => r.GetManyAsync(It.IsAny<Expression<Func<FormTemplate, bool>>>()))
                .ReturnsAsync(new List<FormTemplate> { new FormTemplate { Id = "form1", Name = "Template 1", IsActive = true } });

            // Act
            var result = await _assignmentService.GetAllAsync();

            // Assert
            result.Should().HaveCount(1);
            result[0].AssignedForms.Should().NotBeNull();
            result[0].AssignedForms.Should().HaveCount(1);
            result[0].AssignedForms![0].Id.Should().Be("form1");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAssignment_WhenFound()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            var assignment = new Assignment { Id = id };
            var dto = new AssignmentResponse { Id = id };

            _assignmentRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(assignment);
            _mapperMock.Setup(m => m.Map<AssignmentResponse>(assignment)).Returns(dto);

            // Act
            var result = await _assignmentService.GetByIdAsync(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenNotFound()
        {
            // Arrange
            var id = ObjectId.GenerateNewId().ToString();
            _assignmentRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync((Assignment?)null);

            // Act
            Func<Task> act = async () => await _assignmentService.GetByIdAsync(id);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Asignación no encontrada");
        }

         [Fact]
        public async Task UpdateAssignmentAsync_ShouldUpdateAndReturnResponse()
        {
            // Arrange
            string id = ObjectId.GenerateNewId().ToString();
            var request = new UpdateAssignmentWebRequest { Address = "New Address" };
            var assignment = new Assignment { Id = id };
            var geoPoint = new GeoPoint { Latitude = 5, Longitude = 5 };
            var responseDto = new AssignmentResponse { Id = id };

            _assignmentRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(assignment);
            _geocodingServiceMock.Setup(s => s.GetGeoPointAsync(request.Address)).ReturnsAsync(geoPoint);
            _mapperMock.Setup(m => m.Map<AssignmentResponse>(assignment)).Returns(responseDto);

            // Act
            var result = await _assignmentService.UpdateAssignmentAsync(id, request);

            // Assert
            _assignmentRepositoryMock.Verify(r => r.UpdateAsync(id, assignment), Times.Once);
            result.Should().NotBeNull();
            assignment.DestinationLocation.Should().Be(geoPoint);
        }

        [Fact]
        public async Task StartAssignmentAsync_ShouldSetCheckIn_WhenNotSet()
        {
             // Arrange
            string id = ObjectId.GenerateNewId().ToString();
            var request = new StartAssignmentRequest { CheckIn = DateTime.UtcNow, CurrentLocation = new GeoPoint() };
            var assignment = new Assignment { Id = id, CheckIn = null };
            var responseDto = new AssignmentMobileResponse();

            _assignmentRepositoryMock.Setup(r => r.GetAsync(id)).ReturnsAsync(assignment);
            _mapperMock.Setup(m => m.Map<AssignmentMobileResponse>(assignment)).Returns(responseDto);

            // Act
            await _assignmentService.StartAssignmentAsync(id, request);

            // Assert
            assignment.CheckIn.Should().Be(request.CheckIn);
            assignment.CurrentLocation.Should().Be(request.CurrentLocation);
            _assignmentRepositoryMock.Verify(r => r.UpdateAsync(id, assignment), Times.Once);
        }
    }
}
