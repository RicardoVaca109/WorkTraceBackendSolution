using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
[Authorize]
public class EvaluationDashboardController(IEvaluationDashboardService dashboardService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] string userId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        return Ok(
            await dashboardService.GetDashboardAsync(userId, start, end)
        );
    }
}