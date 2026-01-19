using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AssignmentEvaluationController(IAssignmentEvaluationService _service) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAssignmentEvaluationRequest request)
    {
        var result = await _service.CreateEvaluationAsync(request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("active/{assignmentId}")]
    public async Task<IActionResult> GetActive(string assignmentId)
    {
        var result = await _service.GetActiveByAssignmentAsync(assignmentId);

        if (result == null)
            return NotFound("No existe una evaluación activa para esta asignación.");

        return Ok(result);
    }

    [Authorize]
    [HttpGet("history/{assignmentId}")]
    public async Task<IActionResult> GetHistory(string assignmentId)
    {
        var result = await _service.GetHistoryByAssignmentAsync(assignmentId);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{evaluationId}")]
    public async Task<IActionResult> Update(
        string evaluationId,
        [FromBody] UpdateAssignmentEvaluationRequest request)
    {
        await _service.UpdateEvaluationAsync(evaluationId, request);
        return NoContent();
    }
}