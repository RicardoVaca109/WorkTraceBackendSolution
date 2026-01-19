using MongoDB.Bson;
using MongoDB.Driver;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class ClientRepository : GenericRepository<Client>, IClientRepository
{
    public ClientRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<Client>("clients");
    }

    public async Task<List<Client>> GetClientsByFullNameAsync(string fullName)
    {
        var filter = Builders<Client>.Filter.Regex(
            x => x.FullName,
            new BsonRegularExpression($"^{fullName}", "i")
        );
        return await Collection.Find(filter).ToListAsync();
    }

    public async Task<Client> GetByDocumentNumberAsync(string documentNumber) =>
        await Collection.Find(x => x.DocumentNumber == documentNumber).FirstOrDefaultAsync();

    public async Task<Client> GetByEmailAsync(string email) =>
        await Collection.Find(x => x.Email == email).FirstOrDefaultAsync();

    public async Task<Client> GetByPhoneNumberAsync(string phoneNumber) =>
        await Collection.Find(x => x.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
}