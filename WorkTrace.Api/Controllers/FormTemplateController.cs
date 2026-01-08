using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkTrace.Application.DTOs.FormTemplateDTO.Information;
using WorkTrace.Application.Services;
using WorkTrace.Logic.Services;

namespace WorkTrace.Api.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class FormTemplateController(IFormTemplateService formTemplateService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFormTemplateRequest request)
    {
        var response = await formTemplateService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await formTemplateService.GetAllAsync();
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await formTemplateService.GetByIdAsync(id);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        string id,
        [FromBody] UpdateFormTemplateRequest request)
    {
        var response = await formTemplateService.UpdateAsync(id, request);
        return Ok(response);
    }

    [Authorize]
    [HttpPut("{id}/questions")]
    public async Task<IActionResult> UpdateQuestions(
        string id,
        [FromBody] List<UpdateFormQuestionsRequest> questions)
    {
        var response = await formTemplateService.UpdateQuestionsAsync(id, questions);
        return Ok(response);
    }

    [Authorize]
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(string id)
    {
        var result = await formTemplateService.ActivateAsync(id);
        return Ok(new { Activated = result });
    }

    [Authorize]
    [HttpPatch("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(string id)
    {
        var result = await formTemplateService.DeactivateAsync(id);
        return Ok(new { Deactivated = result });
    }
}
