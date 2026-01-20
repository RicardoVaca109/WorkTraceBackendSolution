using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.DTOs.ClientSessionDTO;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ClientEvaluationController(IClientEvaluationService clientEvaluationService) : ControllerBase
{
    [Authorize]
    [HttpPost("session/{assignmentId}")]
    public async Task<IActionResult> CreateSession(string assignmentId)
    {
        var result = await clientEvaluationService.CreateSessionAsync(assignmentId);
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(ClientEvaluationLoginRequest request)
    => Ok(await clientEvaluationService.LoginAsync(request));

    [HttpPost("submit")]
    [ClientEvaluationAuthorize]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Submit(
    [FromForm] CreateAssignmentEvaluationRequest request)
    {
        var session = HttpContext.Items["ClientSession"] as ClientEvaluationSession;
        await clientEvaluationService.SubmitEvaluationAsync(request, session!);
        return Ok();
    }
}