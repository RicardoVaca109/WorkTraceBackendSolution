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
}