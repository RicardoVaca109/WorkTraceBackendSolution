using Microsoft.AspNetCore.Http;
using WorkTrace.Application.DTOs.ProductInventoryDTO;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Services;

public interface IProductInventoryService
{
    Task ImportInventoryAsync(IFormFile file);
    Task<ProductInventory?> GetByIdAsync(string id);
    Task<List<ProductInventory>> GetAllAsync();
    Task<(List<ProductInventory> Items, long TotalCount)> GetPaginatedAsync(int page, int pageSize, string search);
    Task UpdateAsync(string id, UpdateProductInventoryRequest request);
}
