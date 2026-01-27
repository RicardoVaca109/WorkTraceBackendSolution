using System.Linq.Expressions;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Repositories;

public interface IFormTemplateRepository : IGenericRepository<FormTemplate>
{
    Task<List<FormTemplate>> GetManyAsync(Expression<Func<FormTemplate, bool>> filter);
}