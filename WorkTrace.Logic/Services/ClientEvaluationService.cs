using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.DTOs.ClientSessionDTO;
using WorkTrace.Application.DTOs.FormTemplateDTO.Information;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class ClientEvaluationService(
    IAssignmentRepository assignmentRepository, 
    IUserRepository userRepository, 
    IFormTemplateRepository formTemplateRepository, 
    IClientRepository clientRepository, 
    IClientEvaluationSessionRepository clientEvaluationSessionRepository, 
    IAssignmentEvaluationRepository assignmentEvaluationRepository, 
    IFileService fileService, 
    IConfiguration configuration)
    : IClientEvaluationService
{
    private readonly string _publicBaseUrl = configuration["QREvaluationSettings:PublicBaseUrl"]
        ?? throw new Exception("QREvaluationSettings:PublicBaseUrl no configurado");
    public async Task<CreateEvaluationSessionResponse> CreateSessionAsync(string assignmentId)
    {
        var assignment = await assignmentRepository.GetAsync(assignmentId)
            ?? throw new Exception("Assignment no encontrado");

        var token = Guid.NewGuid().ToString("N");

        var session = new ClientEvaluationSession
        {
            AssignmentId = ObjectId.Parse(assignment.Id),
            ClientId = assignment.Client,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(45),
            IsAuthenticated = false,
            IsUsed = false
        };

        await clientEvaluationSessionRepository.CreateAsync(session);

        return new CreateEvaluationSessionResponse
        {
            Token = token,
            Url = $"{_publicBaseUrl}/HtmlTemplate/login.html?token={token}"
        };
    }

    public async Task<ClientEvaluationLoginResponse> LoginAsync(ClientEvaluationLoginRequest request)
    {
        var session = await clientEvaluationSessionRepository.GetByTokenAsync(request.Token)
            ?? throw new Exception("Sesión inválida o expirada");

        var client = await clientRepository.GetByEmailAsync(request.Email)
            ?? throw new Exception("Credenciales inválidas");

        if (client.DocumentNumber != request.DocumentNumber)
            throw new Exception("Credenciales inválidas");

        if (ObjectId.Parse(client.Id) != session.ClientId)
            throw new Exception("Cliente no pertenece a esta sesión");

        session.IsAuthenticated = true;
        await clientEvaluationSessionRepository.UpdateAsync(session.Id, session);

        return new ClientEvaluationLoginResponse
        {
            SessionToken = session.Token,
            AssignmentId = session.AssignmentId.ToString(),
            ExpiresAt = session.ExpiresAt
        };
    }

    public async Task SubmitEvaluationAsync(
    CreateAssignmentEvaluationRequest request,
    ClientEvaluationSession session)
    {
        if (session.IsUsed)
            throw new Exception("Sesión ya utilizada");

        MediaFile? signatureFile = null;

        if (request.ClientSignature != null)
        {
            var url = await fileService.SaveFileAsync(
                request.ClientSignature,
                "client-signatures");

            signatureFile = new MediaFile
            {
                Url = url,
                UploadedAt = DateTime.UtcNow
            };
        }

        var evaluation = new AssignmentEvaluation
        {
            AssignmentId = session.AssignmentId,
            FormTemplateId = ObjectId.Parse(request.FormType),
            UserEvaluations = request.UserEvaluations.Select(u => new UserEvaluation
            {
                UserId = ObjectId.Parse(u.UserId),
                Answers = u.Answers.Select(a => new FormAnswers
                {
                    QuestionKey = a.QuestionKey,
                    QuestionValue = a.QuestionValue,
                    AnswerType = a.AnswerType,
                    Answer = a.Answer,
                    NumericValue = a.NumericValue
                }).ToList()
            }).ToList(),
            ClientComment = request.ClientComment,
            ClientSignature = new ClientSignature
            {
                Signature = signatureFile,
                SignedBy = request.SignedBy,
                SignedAt = DateTime.UtcNow
            },
            CreatedAt = DateTime.UtcNow
        };

        await assignmentEvaluationRepository.CreateAsync(evaluation);

        session.IsUsed = true;
        await clientEvaluationSessionRepository.UpdateAsync(session.Id, session);
    }
    public async Task<AssignmentEvaluationFormResponse> GetEvaluationFormAsync(
    string assignmentId)
    {
        var assignment = await assignmentRepository.GetAsync(assignmentId)
            ?? throw new Exception("Assignment no encontrado");

        var technicianIds = assignment.Users
            .Select(u => u.ToString())
            .ToList();

        var technicians = new List<User>();

        foreach (var techId in technicianIds)
        {
            var user = await userRepository.GetAsync(techId);
            if (user != null)
                technicians.Add(user);
        }

        var formTemplateIds = assignment.AssignedForms?
            .Select(ObjectId.Parse)
            .ToList() ?? new List<ObjectId>();

        var formTemplateStringIds = formTemplateIds.Select(id => id.ToString()).ToList();

        var formTemplates = await formTemplateRepository.GetManyAsync(
            f => formTemplateStringIds.Contains(f.Id)
        );

        var existingEvaluations =
            await assignmentEvaluationRepository.GetByAssignmentAsync(
                ObjectId.Parse(assignment.Id)
            );

        var evaluations = new List<TechnicianEvaluationFormResponse>();

        foreach (var technician in technicians)
        {
            foreach (var form in formTemplates)
            {
                var isCompleted = existingEvaluations.Any(e =>
                    e.FormTemplateId == ObjectId.Parse(form.Id) &&
                    e.UserEvaluations.Any(ue =>
                        ue.UserId == ObjectId.Parse(technician.Id))
                );

                evaluations.Add(new TechnicianEvaluationFormResponse
                {
                    TechnicianId = technician.Id,
                    TechnicianName = technician.FullName,

                    FormTemplateId = form.Id,
                    FormName = form.Name,

                    IsCompleted = isCompleted,

                    Questions = form.Questions.Select(q => new FormQuestionResponse
                    {
                        Id = q.Id.ToString(),
                        QuestionKey = q.QuestionKey,
                        QuestionText = q.QuestionText,
                        AnswerType = q.AnswerType
                    }).ToList()
                });
            }
        }
        return new AssignmentEvaluationFormResponse
        {
            AssignmentId = assignment.Id,
            ClientId = assignment.Client.ToString(),
            Evaluations = evaluations
        };
    }
}