using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.IdGenerators;
using WorkTrace.Application.DTOs.AssignmentDTO.Management;
using WorkTrace.Application.Services;
using WorkTrace.Logic.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AssignmentController(IAssignmentService assignmentService) : Controller
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AssignmentResponse>> Create([FromBody] CreateAssignmentRequest request)
    {
        var result = await assignmentService.CreateAssigmentAdminAsync(request);
        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<AssignmentResponse>>> GetAll()
    {
        var assignments = await assignmentService.GetAllAsync();
        return Ok(assignments);
    }

    [Authorize]
    [HttpGet]
    public async Task<AssignmentResponse> GetById(string id) =>
        await assignmentService.GetByIdAsync(id);

    [Authorize]
    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClientHistory(string clientId)
    {
        var clientHistory = await assignmentService.GetClientHistoryAsync(clientId);
        return Ok(clientHistory);
    }
}
