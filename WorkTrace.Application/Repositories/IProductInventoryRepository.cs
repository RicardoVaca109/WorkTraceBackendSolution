using WorkTrace.Data.Models;

namespace WorkTrace.Application.Repositories;

public interface IProductInventoryRepository : IGenericRepository<ProductInventory>
{
    Task BulkUpsertAsync(List<ProductInventory> products);
    Task<(List<ProductInventory> Items, long TotalCount)> GetPaginatedAsync(int page, int pageSize, string search);
}