using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.TakenRequirementDTO;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class TakenRequirementsController(ITakenRequirementService takenRequirementService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<TakenRequirementInformationResponse>>> GetAll()
    {
        var result = await takenRequirementService.GetAllAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<TakenRequirementInformationResponse>> GetById(string id)
    {
        try
        {
            var result = await takenRequirementService.GetByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<TakenRequirementInformationResponse>> Create(
        [FromBody] CreateTakenRequirementRequest request)
    {
        try
        {
            var result = await takenRequirementService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<TakenRequirementInformationResponse>> Update(
    string id,
    [FromBody] UpdateTakenRequirementRequest request)
    {
        var updated = await takenRequirementService.UpdateAsync(id, request);
        return Ok(updated);
    }

    [Authorize]
    [HttpGet("user-taken-requirement/{userId}")]
    public async Task<IActionResult> GetByUserAndDateRange(
        string userId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        var result = await takenRequirementService
            .GetByUserAndDateRangeAsync(userId, start, end);

        return Ok(result);
    }
}