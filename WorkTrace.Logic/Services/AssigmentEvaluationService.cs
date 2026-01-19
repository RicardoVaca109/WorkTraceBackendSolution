using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.Enums;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class AssigmentEvaluationService(IAssignmentEvaluationRepository evaluationRepository, IMapper mapper) : IAssignmentEvaluationService
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
}