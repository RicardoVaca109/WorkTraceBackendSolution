using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class MediaFileRepository : GenericRepository<MediaFile>
{
    public MediaFileRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<MediaFile>("mediaFiles");
    }
}
