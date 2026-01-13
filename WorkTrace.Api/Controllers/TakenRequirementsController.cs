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
    [HttpPut]
    public async Task<ActionResult<TakenRequirementInformationResponse>> Update(
        [FromBody] UpdateTakenRequirementRequest request)
    {
        try
        {
            var result = await takenRequirementService.UpdateAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}