using MongoDB.Bson;
using WorkTrace.Application.DTOs.KPIsDTO;
using WorkTrace.Application.Enums;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;

namespace WorkTrace.Logic.Services;

public class EvaluationDashboardService(IAssignmentEvaluationRepository assignmentEvaluationRepository,
    IUserRepository userRepository) : IEvaluationDashboardService
{
    public async Task<EvaluationDashboardResponse> GetDashboardAsync(string userId, DateTime start, DateTime end)
    {
        var user = await userRepository.GetAsync(userId)
            ?? throw new Exception("Usuario no encontrado");

        if (user.Role != UserRoles.Técnico &&
            user.Role != UserRoles.Vendedor)
            throw new Exception("Rol no permitido");

        var evaluations =
            await assignmentEvaluationRepository
                .GetByDateRangeAsync(start, end);

        var userObjectId = ObjectId.Parse(user.Id);

        var userEvaluations = evaluations
            .SelectMany(e => e.UserEvaluations)
            .Where(ue => ue.UserId == userObjectId)
            .ToList();

        var numericAnswers = userEvaluations
            .SelectMany(ue => ue.Answers)
            .Where(a =>
                a.AnswerType == AnswerType.Numeric &&
                a.NumericValue.HasValue
            );

        var metrics = numericAnswers
            .GroupBy(a => new
            {
                a.QuestionKey,
                a.QuestionValue
            })
            .Select(g => new DashboardQuestionMetricResponse
            {
                QuestionKey = g.Key.QuestionKey,
                QuestionText = g.Key.QuestionValue,
                Average = Math.Round(
                    g.Average(x => x.NumericValue!.Value), 2),
                Count = g.Count()
            })
            .ToList();

        return new EvaluationDashboardResponse
        {
            UserId = user.Id,
            UserName = user.FullName,
            TotalEvaluations = userEvaluations.Count,
            Metrics = metrics
        };
    }
}