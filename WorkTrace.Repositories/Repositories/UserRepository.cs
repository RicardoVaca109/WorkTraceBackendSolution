using MongoDB.Driver;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<User>("users");
    }

    public async Task<User> GetByDocumentNumberAsync(string documentNumber) =>
        await Collection.Find(x => x.DocumentNumber == documentNumber).FirstOrDefaultAsync();

    public async Task<User?> GetByEmailAsync(string email) =>
        await Collection.Find(x => x.Email == email).FirstOrDefaultAsync();
}