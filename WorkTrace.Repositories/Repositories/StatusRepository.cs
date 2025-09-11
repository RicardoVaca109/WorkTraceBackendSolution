
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

internal class StatusRepository : GenericRepository<Status>, IStatusRepository
{
    public StatusRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<Status>("status");
    }
}
