using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WorkTrace.Application.DTOs.SalesEvaluationDTO;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalesEvaluationController(ISalesEvaluationService salesEvaluationService) : ControllerBase
{
    [Authorize]
    [HttpPost("submit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Submit([FromForm] CreateSalesEvaluationRequest request)
    {
        var clientId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(clientId))
        {
            return Unauthorized();
        }

        await salesEvaluationService.SubmitEvaluationAsync(request, clientId);
        return Ok();
    }
}