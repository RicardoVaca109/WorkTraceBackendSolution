using WorkTrace.Data.Models;

namespace WorkTrace.Application.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User> GetByDocumentNumberAsync(string documentNumber);
    Task<User?> GetByEmailAsync(string email);
}