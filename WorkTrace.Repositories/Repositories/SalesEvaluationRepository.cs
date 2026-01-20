using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class SalesEvaluationRepository : GenericRepository<SalesEvaluation>, ISalesEvaluationRepository
{
    public SalesEvaluationRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<SalesEvaluation>("salesEvaluations");
    }
}