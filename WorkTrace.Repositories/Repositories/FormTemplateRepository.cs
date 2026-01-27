using MongoDB.Driver;
using System.Linq.Expressions;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class FormTemplateRepository : GenericRepository<FormTemplate>, IFormTemplateRepository
{
    public FormTemplateRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<FormTemplate>("formTemplates");
    }

    public async Task<List<FormTemplate>> GetManyAsync(Expression<Func<FormTemplate, bool>> filter)
    {
        return await Collection
            .Find(filter)
            .ToListAsync();
    }
}