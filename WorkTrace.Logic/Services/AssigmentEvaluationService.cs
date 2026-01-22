using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.Enums;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class AssigmentEvaluationService(IAssignmentEvaluationRepository evaluationRepository, IAssignmentRepository assignmentRepository, IUserRepository userRepository, IFormTemplateRepository formTemplateRepository, IMapper mapper) : IAssignmentEvaluationService
{
    public async Task<AssignmentEvaluationResponse> CreateEvaluationAsync(CreateAssignmentEvaluationRequest request)
    {
        var assignmentId = ObjectId.Parse(request.AssignmentId);

        var active = await evaluationRepository
            .GetByAssignmentAsync(assignmentId);

        var evaluation = mapper.Map<AssignmentEvaluation>(request);
        evaluation.CreatedAt = DateTime.UtcNow;

        foreach (var user in evaluation.UserEvaluations)
        {
            user.Score = CalculateScore(user.Answers);
        }

        await evaluationRepository.CreateAsync(evaluation);

        return mapper.Map<AssignmentEvaluationResponse>(evaluation);
    }

    public async Task<AssignmentEvaluationResponse?> GetActiveByAssignmentAsync(string assignmentId)
    {
        var evaluation = await evaluationRepository
            .GetByAssignmentAsync(ObjectId.Parse(assignmentId));

        return evaluation == null
            ? null
            : mapper.Map<AssignmentEvaluationResponse>(evaluation);
    }

    public async Task<List<AssignmentEvaluationResponse>> GetHistoryByAssignmentAsync(string assignmentId)
    {
        var list = await evaluationRepository.GetAsync();

        var result = list
            .Where(x => x.AssignmentId == ObjectId.Parse(assignmentId))
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        return mapper.Map<List<AssignmentEvaluationResponse>>(result);
    }

    public async Task UpdateEvaluationAsync(string evaluationId, UpdateAssignmentEvaluationRequest request)
    {
        var evaluation = await evaluationRepository.GetAsync(evaluationId);

        if (evaluation == null)
            throw new Exception("Evaluación no encontrada");

        evaluation.UserEvaluations =
            mapper.Map<List<UserEvaluation>>(request.UserEvaluations);

        foreach (var user in evaluation.UserEvaluations)
        {
            user.Score = CalculateScore(user.Answers);
        }

        await evaluationRepository.UpdateAsync(evaluationId, evaluation);
    }

    private int CalculateScore(List<FormAnswers> answers)
    {
        var numeric = answers
            .Where(x => x.AnswerType == AnswerType.Numeric &&
                        x.NumericValue.HasValue)
            .Select(x => x.NumericValue!.Value)
            .ToList();

        if (!numeric.Any())
            return 0;

        return (int)Math.Round(numeric.Average(), 0);
    }

    public async Task<AssignmentEvaluationDetailResponse>GetEvaluationDetailByAssignmentAsync(string assignmentId)
    {
        var assignmentObjectId = ObjectId.Parse(assignmentId);

        // 1. Obtener la asignación
        var assignment = await assignmentRepository.GetAsync(assignmentId)
            ?? throw new Exception("Asignación no encontrada");

        // 2. Obtener evaluaciones de la asignación
        var evaluations = await evaluationRepository
            .GetByAssignmentAsync(assignmentObjectId);

        if (!evaluations.Any())
            throw new Exception("No existen evaluaciones para esta asignación");

        // 3. Obtener formularios
        var formTemplateIds = evaluations
            .Select(e => e.FormTemplateId.ToString())
            .Distinct()
            .ToList();

        var formTemplates = await formTemplateRepository.GetManyAsync(
            f => formTemplateIds.Contains(f.Id)
        );

        // 4. Obtener técnicos evaluados
        var userIds = evaluations
            .SelectMany(e => e.UserEvaluations)
            .Select(u => u.UserId.ToString())
            .Distinct()
            .ToList();

        var users = new Dictionary<string, User>();

        foreach (var userId in userIds)
        {
            var user = await userRepository.GetAsync(userId);
            if (user != null)
                users[userId] = user;
        }

        // 5. Construir respuesta
        var response = new AssignmentEvaluationDetailResponse
        {
            AssignmentId = assignmentId,
            ClientComment = evaluations.First().ClientComment,
            ClientSignature = evaluations.First().ClientSignature == null
                ? null
                : new ClientSignatureResponse
                {
                    Url = evaluations.First().ClientSignature.Signature.Url,
                    SignedBy = evaluations.First().ClientSignature.SignedBy,
                    SignedAt = evaluations.First().ClientSignature.SignedAt
                },
            MediaFiles = assignment.MediaFiles?
                .Select(m => new MediaFileResponse
                {
                    Url = m.Url,
                    UploadedAt = m.UploadedAt
                }).ToList() ?? new()
        };

        // 6. Agrupar por formulario
        foreach (var form in formTemplates)
        {
            var formEvaluations = evaluations
                .Where(e => e.FormTemplateId == ObjectId.Parse(form.Id))
                .ToList();

            var formResponse = new FormEvaluationResponse
            {
                FormTemplateId = form.Id,
                FormName = form.Name,
                Questions = form.Questions.Select(q => new FormEvaluationQuestionResponse
                {
                    QuestionKey = q.QuestionKey,
                    QuestionText = q.QuestionText
                }).ToList()
            };

            foreach (var eval in formEvaluations)
            {
                foreach (var userEval in eval.UserEvaluations)
                {
                    var userId = userEval.UserId.ToString();

                    formResponse.UserEvaluations.Add(new UserEvaluationResponse
                    {
                        UserId = userId,
                        UserName = users.ContainsKey(userId)
                            ? users[userId].FullName
                            : "Desconocido",
                        Score = userEval.Score,
                        Answers = userEval.Answers.Select(a => new FormAnswerResponse
                        {
                            QuestionKey = a.QuestionKey,
                            QuestionValue = a.QuestionValue,
                            Answer = a.Answer,
                            NumericValue = a.NumericValue
                        }).ToList()
                    });
                }
            }
            response.Forms.Add(formResponse);
        }
        return response;
    }
}