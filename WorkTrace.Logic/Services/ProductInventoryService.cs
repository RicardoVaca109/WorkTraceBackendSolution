using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using WorkTrace.Application.DTOs.ProductInventoryDTO;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class ProductInventoryService : IProductInventoryService
{
    private readonly IProductInventoryRepository _repository;

    public ProductInventoryService(IProductInventoryRepository repository)
    {
        _repository = repository;
    }

    public async Task ImportInventoryAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null.");

        ExcelPackage.License.SetNonCommercialPersonal("DemoUser");

        var products = new List<ProductInventory>();

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0]; // Assume first sheet
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;

                // 1. Find the Header Row
                int headerRow = -1;
                for (int row = 1; row <= Math.Min(20, rowCount); row++) // Search first 20 rows
                {
                    for (int col = 1; col <= colCount; col++)
                    {
                        var cellValue = worksheet.Cells[row, col].Text;
                        if (cellValue != null && cellValue.Contains("Código", StringComparison.OrdinalIgnoreCase))
                        {
                            headerRow = row;
                            break;
                        }
                    }
                    if (headerRow != -1) break;
                }

                if (headerRow == -1)
                {
                    throw new ArgumentException("Could not find header row containing 'Código'.");
                }

                // 2. Map Columns
                int codeCol = -1;
                int descCol = -1;
                int quantityCol = -1;
                int avgCostCol = -1;

                for (int col = 1; col <= colCount; col++)
                {
                    var headerText = worksheet.Cells[headerRow, col].Text?.Trim();
                    if (string.IsNullOrEmpty(headerText)) continue;

                    if (headerText.Contains("Código", StringComparison.OrdinalIgnoreCase)) codeCol = col;
                    else if (headerText.Contains("Producto", StringComparison.OrdinalIgnoreCase)) descCol = col;
                    else if (headerText.Contains("Stock", StringComparison.OrdinalIgnoreCase)) quantityCol = col;
                    else if (headerText.Contains("Costo SIN IVA", StringComparison.OrdinalIgnoreCase)) avgCostCol = col;
                }

                if (codeCol == -1 || descCol == -1 || quantityCol == -1 || avgCostCol == -1)
                {
                    throw new ArgumentException("Missing required columns: 'Código', 'Producto', 'Stock', or 'Costo SIN IVA'.");
                }

                // 3. Parse Data
                for (int row = headerRow + 1; row <= rowCount; row++)
                {
                    var code = worksheet.Cells[row, codeCol].Value?.ToString();
                    if (string.IsNullOrWhiteSpace(code)) continue;

                    var description = worksheet.Cells[row, descCol].Value?.ToString() ?? string.Empty;

                    var quantityText = worksheet.Cells[row, quantityCol].Value?.ToString();
                    int.TryParse(quantityText, out int quantity);

                    var avgCostText = worksheet.Cells[row, avgCostCol].Value?.ToString();
                    decimal.TryParse(avgCostText, out decimal avgCost);

                    // 4. Calculate Total
                    var totalCost = quantity * avgCost;

                    var product = new ProductInventory
                    {
                        Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                        Code = code,
                        Description = description,
                        QuantityBalance = quantity,
                        AverageCostBalance = avgCost,
                        TotalCostBalance = totalCost
                    };

                    products.Add(product);
                }
            }
        }

        foreach (var batch in products.Chunk(1000))
        {
            await _repository.BulkUpsertAsync(batch.ToList());
        }
    }

    public async Task<ProductInventory?> GetByIdAsync(string id)
    {
        return await _repository.GetAsync(id);
    }

    public async Task<List<ProductInventory>> GetAllAsync()
    {
        return await _repository.GetAsync();
    }

    public async Task<(List<ProductInventory> Items, long TotalCount)> GetPaginatedAsync(int page, int pageSize, string search)
    {
        return await _repository.GetPaginatedAsync(page, pageSize, search);
    }

    public async Task UpdateAsync(string id, UpdateProductInventoryRequest request)
    {
        var product = await _repository.GetAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        product.Code = request.Code;
        product.Description = request.Description;
        product.QuantityBalance = request.QuantityBalance;
        product.AverageCostBalance = request.AverageCostBalance;
        product.TotalCostBalance = request.TotalCostBalance;

        await _repository.UpdateAsync(id, product);
    }
}