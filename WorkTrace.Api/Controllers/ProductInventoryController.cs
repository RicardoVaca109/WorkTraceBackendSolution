using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.ProductInventoryDTO;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ProductInventoryController(IProductInventoryService service) : Controller
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ImportInventory(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            await service.ImportInventoryAsync(file);
            return Ok("Inventory imported successfully.");
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CRITICAL ERROR] Import failed: {ex.Message} \n {ex.StackTrace}");
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var product = await service.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound($"Product with ID {id} not found.");
        }
        return Ok(product);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 50, [FromQuery] string search = "")
    {
        var (items, totalItems) = await service.GetPaginatedAsync(page, pageSize, search);
        return Ok(new { totalItems, items });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateProductInventoryRequest request)
    {
        try
        {
            await service.UpdateAsync(id, request);
            return Ok("Product inventory updated successfully.");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
