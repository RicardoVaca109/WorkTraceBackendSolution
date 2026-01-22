using MongoDB.Driver;
using WorkTrace.Application.Repositories;
using WorkTrace.Data;
using WorkTrace.Data.Models;

namespace WorkTrace.Repositories.Repositories;

public class ProductInventoryRepository : GenericRepository<ProductInventory>, IProductInventoryRepository
{
    public ProductInventoryRepository(WorkTraceContext context)
    {
        Collection = context.GetCollection<ProductInventory>("ProductInventory");
    }

    public async Task BulkUpsertAsync(List<ProductInventory> products)
    {
        if (products == null || !products.Any()) return;

        var bulkOps = new List<WriteModel<ProductInventory>>();

        foreach (var product in products)
        {
            var filter = Builders<ProductInventory>.Filter.Eq(x => x.Code, product.Code);

            var update = Builders<ProductInventory>.Update
                .Set(p => p.Description, product.Description)
                .Set(p => p.QuantityBalance, product.QuantityBalance)
                .Set(p => p.AverageCostBalance, product.AverageCostBalance)
                .Set(p => p.TotalCostBalance, product.TotalCostBalance)
                .SetOnInsert(p => p.Id, product.Id);

            var upsertOne = new UpdateOneModel<ProductInventory>(filter, update)
            {
                IsUpsert = true
            };
            bulkOps.Add(upsertOne);
        }
        await Collection.BulkWriteAsync(bulkOps);
    }

    public async Task<(List<ProductInventory> Items, long TotalCount)> GetPaginatedAsync(int page, int pageSize, string search)
    {
        var filter = Builders<ProductInventory>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var regex = new MongoDB.Bson.BsonRegularExpression(search, "i");
            filter = Builders<ProductInventory>.Filter.Or(
                Builders<ProductInventory>.Filter.Regex(x => x.Code, regex),
                Builders<ProductInventory>.Filter.Regex(x => x.Description, regex)
            );
        }

        var totalCount = await Collection.CountDocumentsAsync(filter);

        var items = await Collection.Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}