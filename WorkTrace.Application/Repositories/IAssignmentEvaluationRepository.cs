using MongoDB.Bson;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Repositories;

public interface IAssignmentEvaluationRepository : IGenericRepository<AssignmentEvaluation>
{
    Task<List<AssignmentEvaluation>> GetByAssignmentAsync(ObjectId assignmentId);
    Task<AssignmentEvaluation?> GetByAssignmentAndFormAsync(ObjectId assignmentId, ObjectId formTemplateId);
    Task<List<AssignmentEvaluation>> GetByDateRangeAsync(DateTime start, DateTime end);
}