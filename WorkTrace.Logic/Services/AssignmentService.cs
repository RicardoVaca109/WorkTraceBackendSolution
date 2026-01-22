using AutoMapper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using WorkTrace.Application.DTOs.AssignmentDTO.Management;
using WorkTrace.Application.DTOs.AssignmentDTO.Mobile;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.DTOs.FormTemplateDTO.Information;
using WorkTrace.Application.DTOs.ServiceMgmtDTO.Management;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class AssignmentService(IAssignmentRepository _assignmentRepository, IClientRepository _clientRepository, IFileService fileService, IFormTemplateRepository _formTemplateRepository, IGeocodingService _geocodingService, IInstallationStepRepository _installationStepRepository, IServiceRepository _serviceRepository, IStatusRepository _statusRepository, IUserRepository _userRepository, IMapper _mapper) : IAssignmentService
{
    public async Task<AssignmentResponse> CreateAssignmentAdminAsync(CreateAssignmentRequest request)
    {
        await ValidateExistance(request);

        var assignment = _mapper.Map<Assignment>(request);

        assignment.DestinationLocation =
            await _geocodingService.GetGeoPointAsync(request.Address);

        assignment.AssignedForms ??= new List<string>();

        await _assignmentRepository.CreateAsync(assignment);

        var dto = _mapper.Map<AssignmentResponse>(assignment);
        dto.AssignedForms = await ResolveAssignedFormsAsync(assignment.AssignedForms);
        return dto;
    }

    public async Task<List<AssignmentResponse>> GetAllAsync()
    {
        var assignmentsInSystem = await _assignmentRepository.GetAsync();
        var responseList = _mapper.Map<List<AssignmentResponse>>(assignmentsInSystem);

        foreach (var dto in responseList)
        {
            var original = assignmentsInSystem.FirstOrDefault(a => a.Id.ToString() == dto.Id);
            if (original != null)
            {
                dto.AssignedForms = await ResolveAssignedFormsAsync(original.AssignedForms);
            }
        }

        return responseList;
    }

    public async Task<AssignmentResponse> GetByIdAsync(string id)
    {
        var assignmentById = await _assignmentRepository.GetAsync(id) ?? throw new Exception("Asignación no encontrada");
        var response = _mapper.Map<AssignmentResponse>(assignmentById);
        response.AssignedForms = await ResolveAssignedFormsAsync(assignmentById.AssignedForms);
        return response;
    }

    public async Task<List<ClientHistoryResponse>> GetClientHistoryAsync(string clientId)
    {
        var rawData = await _assignmentRepository.GetClientAssignmentRawAsync(clientId);

        var mapResult = rawData.Select(doc =>
        {
            DateTime assignedDate = DateTime.MinValue;
            if (doc.TryGetValue("AssignedDate", out var assignedVal) && assignedVal != BsonNull.Value)
            {
                var utc = assignedVal.AsBsonDateTime.ToUniversalTime();
                assignedDate = utc.ToLocalTime();
            }

            DateTime? checkIn = null;
            if (doc.TryGetValue("CheckIn", out var checkInVal) && checkInVal != BsonNull.Value)
            {
                var utc = checkInVal.AsBsonDateTime.ToUniversalTime();
                checkIn = utc.ToLocalTime();
            }

            DateTime? checkOut = null;
            if (doc.TryGetValue("CheckOut", out var checkOutVal) && checkOutVal != BsonNull.Value)
            {
                var utc = checkOutVal.AsBsonDateTime.ToUniversalTime();
                checkOut = utc.ToLocalTime();
            }

            var dto = new ClientHistoryResponse
            {
                Service = doc["Service"].AsString,
                AssignedDate = assignedDate,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Status = doc["Status"].AsString,
                Address = doc["Address"].AsString,
                Users = doc["Users"]
                    .AsBsonArray
                    .Select(u => u.AsString)
                    .ToList()
            };

            List<string> assignedFormIds = new();
            if (doc.TryGetValue("AssignedForms", out var formsVal) && formsVal != BsonNull.Value && formsVal.IsBsonArray)
            {
                assignedFormIds = formsVal.AsBsonArray.Select(f => f.ToString()).ToList();
            }

            return (Dto: dto, FormIds: assignedFormIds);
        }).ToList();

        // Resolve forms
        foreach(var item in mapResult)
        {
            item.Dto.AssignedForms = await ResolveAssignedFormsAsync(item.FormIds);
        }

        return mapResult.Select(x => x.Dto).ToList();
    }

    public async Task<AssignmentResponse> UpdateAssignmentAsync(string id, UpdateAssignmentWebRequest request)
    {
        var assignment = await _assignmentRepository.GetAsync(id)
            ?? throw new Exception("Asignación no encontrada");

        _mapper.Map(request, assignment);

        await ProcesarFormsAsync(request, assignment);

        if (!string.IsNullOrEmpty(request.Address))
        {
            assignment.DestinationLocation =
                await _geocodingService.GetGeoPointAsync(request.Address);
        }

        await _assignmentRepository.UpdateAsync(id, assignment);

        var response = _mapper.Map<AssignmentResponse>(assignment);
        response.AssignedForms = await ResolveAssignedFormsAsync(assignment.AssignedForms);
        return response;

    }

    public async Task<List<AssignmentListResponse>> GetAssignmentsForListAsync(string userId)
    {
        var raw = await _assignmentRepository.GetAssignmentsListByUserRawAsync(userId);

        var list = raw.Select(x =>
        {
            var assignedDateLocal = DateTime.MinValue;
            if (x.TryGetValue("AssignedDate", out var assignedVal) && assignedVal != BsonNull.Value)
            {
                if (assignedVal.IsBsonDateTime)
                {
                    var utc = assignedVal.AsBsonDateTime.ToUniversalTime();
                    assignedDateLocal = utc.ToLocalTime();
                }
                else if (assignedVal.IsValidDateTime)
                {
                    var utc = (DateTime)assignedVal;
                    if (utc.Kind == DateTimeKind.Unspecified)
                        utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
                    assignedDateLocal = utc.ToLocalTime();
                }
            }

            string client = "";
            if (x.TryGetValue("Client", out var clientVal) && clientVal != BsonNull.Value)
            {
                client = clientVal.AsString;
            }

            string service = "";
            if (x.TryGetValue("Service", out var serviceVal) && serviceVal != BsonNull.Value)
            {
                service = serviceVal.AsString;
            }

            List<string> assignedFormIds = new();
            if (x.TryGetValue("AssignedForms", out var formsVal) && formsVal != BsonNull.Value && formsVal.IsBsonArray)
            {
                assignedFormIds = formsVal.AsBsonArray.Select(f => f.ToString()).ToList();
            }

            var dto = new AssignmentListResponse
            {
                Id = x["_id"].ToString(),
                Client = client,
                Service = service,
                AssignedDate = assignedDateLocal
            };

            return (Dto: dto, FormIds: assignedFormIds);
        }).ToList();

        foreach (var item in list)
        {
            item.Dto.AssignedForms = await ResolveAssignedFormsAsync(item.FormIds);
        }

        return list.Select(x => x.Dto).ToList();
    }

    public async Task<AssignmentTrackingResponse?> GetAssignmentTrackingAsync(string assignmentId)
    {
        var doc = await _assignmentRepository.GetAssignmentTrackingRawAsync(assignmentId);
        if (doc == null) return null;

        DateTime? checkIn = null;
        if (doc.TryGetValue("CheckIn", out var checkInVal) && checkInVal != BsonNull.Value)
        {
            var utc = checkInVal.AsBsonDateTime.ToUniversalTime();
            checkIn = utc.ToLocalTime();
        }

        DateTime? checkOut = null;
        if (doc.TryGetValue("CheckOut", out var checkOutVal) && checkOutVal != BsonNull.Value)
        {
            var utc = checkOutVal.AsBsonDateTime.ToUniversalTime();
            checkOut = utc.ToLocalTime();
        }

        return new AssignmentTrackingResponse
        {
            Id = doc["_id"].ToString(),
            Client = doc.GetValue("Client").AsString,
            Service = doc.GetValue("Service").AsString,
            Address = doc.GetValue("Address").AsString,
            CheckIn = checkIn,
            CheckOut = checkOut,
            CurrentLocation = doc.GetValue("CurrentLocation").IsBsonNull
                ? null
                : BsonSerializer.Deserialize<GeoPoint>(doc["CurrentLocation"].AsBsonDocument),

            DestinationLocation = doc.GetValue("DestinationLocation").IsBsonNull
                ? null
                : BsonSerializer.Deserialize<GeoPoint>(doc["DestinationLocation"].AsBsonDocument)
        };
    }

    //Mobile
    public async Task<List<AssignmentMobileDashboardResponse>> GetAssignmentByUserandRangeAsync(
    string userId, DateTime start, DateTime end)
    {
        var data = await _assignmentRepository
        .GetAssignmentByUserAndDateRangeAsync(userId, start, end);

        var result = new List<AssignmentMobileDashboardResponse>();

        foreach (var assignment in data)
        {
            var client = await _clientRepository.GetAsync(assignment.Client.ToString());
            var service = await _serviceRepository.GetAsync(assignment.Service.ToString());
            var createdBy = await _userRepository.GetAsync(assignment.CreatedByUser.ToString());
            var status = await _statusRepository.GetAsync(assignment.Status.ToString());

            var assignedForms = await ResolveAssignedFormsAsync(
                assignment.AssignedForms);

            var dto = new AssignmentMobileDashboardResponse
            {
                Id = assignment.Id.ToString(),
                Client = client?.FullName ?? "Sin nombre",
                Service = service?.Name ?? "Sin nombre",
                Status = status?.Name ?? "Sin nombre",
                Address = assignment.Address,
                AssignedDate = assignment.AssignedDate.ToLocalTime(),
                CreatedByUser = createdBy?.FullName ?? "N/A",
                CheckIn = assignment.CheckIn?.ToLocalTime(),
                CheckOut = assignment.CheckOut?.ToLocalTime(),
                AssignedForms = assignedForms
            };

            result.Add(dto);
        }

        return result;
    }

    public async Task<AssignmentMobileResponse> StartAssignmentAsync(string id, StartAssignmentRequest request)
    {
        var assignment = await _assignmentRepository.GetAsync(id)
        ?? throw new Exception("Asignación no encontrada");

        if (assignment.CheckIn == null)
            assignment.CheckIn = request.CheckIn;

        assignment.CurrentLocation = request.CurrentLocation;

        await _assignmentRepository.UpdateAsync(id, assignment);

        return await GetAssignmentMobileDetailAsync(id);
    }

    public async Task<AssignmentMobileResponse> FinishAssignmentAsync(string id, FinishAssignmentRequest request)
    {
        var assignment = await _assignmentRepository.GetAsync(id)
        ?? throw new Exception("Asignación no encontrada");

        assignment.CheckOut = request.CheckOut;

        await _assignmentRepository.UpdateAsync(id, assignment);

        return await GetAssignmentMobileDetailAsync(id);
    }

    public async Task<AssignmentMobileResponse> UpdateLocationAsync(string id, UpdateLocationRequest request)
    {
        var assignment = await _assignmentRepository.GetAsync(id)
        ?? throw new Exception("Asignación no encontrada");

        assignment.CurrentLocation = request.CurrentLocation;

        await _assignmentRepository.UpdateAsync(id, assignment);

        return await GetAssignmentMobileDetailAsync(id);
    }

    public async Task<AssignmentMobileResponse> UpdateProgressAsync(string id, UpdateProgressRequest request)
    {
        var assignment = await _assignmentRepository.GetAsync(id)
        ?? throw new Exception("Asignación no encontrada");

        if (request.MediaFiles != null)
        {
            assignment.MediaFiles ??= new List<MediaFile>();

            foreach (var file in request.MediaFiles)
            {
                var path = await fileService.SaveFileAsync(file, "uploads/media");

                assignment.MediaFiles.Add(new MediaFile
                {
                    Url = path,
                    UploadedAt = DateTime.UtcNow
                });
            }
        }

        assignment.Comment = request.Comment;

        await _assignmentRepository.UpdateAsync(id, assignment);

        return await GetAssignmentMobileDetailAsync(id);
    }

    public async Task<AssignmentMobileResponse> GetAssignmentMobileDetailAsync(string id)
    {
        var assignment = await _assignmentRepository.GetAsync(id)
            ?? throw new Exception("Asignación no encontrada");

        var response = _mapper.Map<AssignmentMobileResponse>(assignment);

        response.AssignedForms =
            await ResolveAssignedFormsAsync(assignment.AssignedForms);

        return response;
    }

    public async Task<StartAssignmentDetailResponse>GetStartAssignmentDetailAsync(string assignmentId)
    {
        var assignment = await _assignmentRepository.GetAsync(assignmentId)
            ?? throw new Exception("Asignación no encontrada");

        var service = await _serviceRepository.GetAsync(assignment.Service.ToString())
            ?? throw new Exception("Servicio no encontrado");

        var installationStepIds = service.InstallationSteps?
            .Select(x => x.ToString())
            .ToList() ?? new();

        var allSteps = await _installationStepRepository.GetAsync();

        var installationSteps = allSteps
            .Where(s => installationStepIds.Contains(s.Id))
            .ToList();

        var response = new StartAssignmentDetailResponse
        {
            AssignmentId = assignment.Id,

            ServiceName = service.Name,
            ServiceDescription = service.Description,

            InstallationSteps = installationSteps
                .OrderBy(s => s.Steps)
                .Select(s => new InstallationStepResponse
                {
                    Id = s.Id,
                    Steps = s.Steps,
                    Description = s.Description
                })
                .ToList(),

            TechnicianComment = assignment.Comment,

            MediaFiles = assignment.MediaFiles?
                .Select(m => new MediaFileResponse
                {
                    Url = m.Url,
                    UploadedAt = m.UploadedAt
                })
                .ToList() ?? new()
        };

        return response;
    }

    public async Task ValidateExistance(CreateAssignmentRequest assignmentRequest)
    {
        var client = await _clientRepository.GetAsync(assignmentRequest.Client);
        if (client == null) throw new Exception("Cliente no existe");

        var service = await _serviceRepository.GetAsync(assignmentRequest.Service);
        if (service == null) throw new Exception("Servicio no existe");

        var status = await _statusRepository.GetAsync(assignmentRequest.Status);
        if (status == null) throw new Exception("Status no existe");

        foreach (var userId in assignmentRequest.Users)
        {
            var user = await _userRepository.GetAsync(userId);
            if (user == null) throw new Exception($"Usuario no existe");
        }
    }
    private async Task ProcesarFormsAsync(
    UpdateAssignmentWebRequest request,
    Assignment assignment)
    {
        assignment.AssignedForms ??= new List<string>();

        if (request.AddForms != null && request.AddForms.Any())
        {
            foreach (var formId in request.AddForms)
            {
                var form = await _formTemplateRepository.GetAsync(formId);
                if (form == null || !form.IsActive)
                    throw new Exception("Formulario no existe o está inactivo");

                if (!assignment.AssignedForms.Contains(formId))
                    assignment.AssignedForms.Add(formId);
            }
        }

        if (request.RemoveForms != null && request.RemoveForms.Any())
        {
            assignment.AssignedForms.RemoveAll(f =>
                request.RemoveForms.Contains(f));
        }
    }

    private async Task<List<AssignedFormResponse>> ResolveAssignedFormsAsync(
    List<string>? formIds)
    {
        if (formIds == null || !formIds.Any())
            return new();

        var forms = await _formTemplateRepository
            .GetManyAsync(f => formIds.Contains(f.Id) && f.IsActive);

        return forms.Select(f => new AssignedFormResponse
        {
            Id = f.Id,
            Name = f.Name
        }).ToList();
    }
}