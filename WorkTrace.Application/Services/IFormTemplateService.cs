using WorkTrace.Application.DTOs.FormTemplateDTO.Information;

namespace WorkTrace.Application.Services;

public interface IFormTemplateService
{
    Task<List<FormTemplateResponse>> GetAllAsync();
    Task<FormTemplateResponse> GetByIdAsync(string id);
    Task<FormTemplateResponse> CreateAsync(CreateFormTemplateRequest request);
    Task<FormTemplateResponse> UpdateAsync(string id, UpdateFormTemplateRequest request);
    Task<FormTemplateResponse> UpdateQuestionsAsync(string id, List<UpdateFormQuestionsRequest> questions);
    Task<bool> ActivateAsync(string id);
    Task<bool> DeactivateAsync(string id);
}