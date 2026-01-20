using MongoDB.Bson;
using MongoDB.Driver;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class AssignmentEvaluationRepository : GenericRepository<AssignmentEvaluation>, IAssignmentEvaluationRepository
{
    public AssignmentEvaluationRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<AssignmentEvaluation>("assignmentEvaluations");
    }

    public async Task<List<AssignmentEvaluation>> GetByAssignmentAsync(ObjectId assignmentId)
    {
        return await Collection.Find(x =>
            x.AssignmentId == assignmentId
        ).ToListAsync();
    }

    public async Task<AssignmentEvaluation?> GetByAssignmentAndFormAsync(ObjectId assignmentId, ObjectId formTemplateId)
    {
        return await Collection.Find(x =>
            x.AssignmentId == assignmentId &&
            x.FormTemplateId == formTemplateId
        ).FirstOrDefaultAsync();
    }

    public async Task<List<AssignmentEvaluation>>GetByDateRangeAsync(DateTime start, DateTime end)
    {
        return await Collection.Find(x =>
            x.CreatedAt >= start &&
            x.CreatedAt <= end
        ).ToListAsync();
    }
}