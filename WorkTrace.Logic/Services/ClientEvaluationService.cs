using Microsoft.Extensions.Options;
using MongoDB.Bson;
using WorkTrace.Application.Configurations;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.DTOs.ClientSessionDTO;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class ClientEvaluationService(IAssignmentRepository assignmentRepository, IClientRepository clientRepository, IClientEvaluationSessionRepository clientEvaluationSessionRepository, IAssignmentEvaluationRepository assignmentEvaluationRepository, IOptions<QREvaluationSettings> settings)
    : IClientEvaluationService
{
    private readonly QREvaluationSettings _settings = settings.Value;
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
            Url = $"{_settings.PublicBaseUrl}/HtmlTemplate/login.html?token={token}"
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
            ExpiresAt = session.ExpiresAt
        };
    }

    public async Task SubmitEvaluationAsync(
        CreateAssignmentEvaluationRequest request,
        ClientEvaluationSession session)
    {
        if (session.IsUsed)
            throw new Exception("Sesión ya utilizada");

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
                SignatureBase64 = request.ClientSignature.SignatureBase64,
                SignedBy = request.ClientSignature.SignedBy,
                SignedAt = DateTime.UtcNow
            },
            CreatedAt = DateTime.UtcNow
        };

        await assignmentEvaluationRepository.CreateAsync(evaluation);

        session.IsUsed = true;
        await clientEvaluationSessionRepository.UpdateAsync(session.Id, session);
    }
}