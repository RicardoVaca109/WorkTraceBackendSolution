using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.FormTemplateDTO.Information;
using WorkTrace.Application.Repositories;
using WorkTrace.Application.Services;
using WorkTrace.Data.Models;

namespace WorkTrace.Logic.Services;

public class FormTemplateService (IFormTemplateRepository _formTemplateRepository, IMapper _mapper) : IFormTemplateService
{
    public async Task<List<FormTemplateResponse>> GetAllAsync()
    {
        var templates = await _formTemplateRepository.GetAsync();
        return _mapper.Map<List<FormTemplateResponse>>(templates);
    }

    public async Task<FormTemplateResponse> GetByIdAsync(string id)
    {
        var template = await _formTemplateRepository.GetAsync(id);
        if (template == null)
            throw new Exception("Plantilla no encontrada");

        return _mapper.Map<FormTemplateResponse>(template);
    }

    public async Task<FormTemplateResponse> CreateAsync(CreateFormTemplateRequest request)
    {
        var template = _mapper.Map<FormTemplate>(request);
        template.IsActive = true;

        await _formTemplateRepository.CreateAsync(template);

        return _mapper.Map<FormTemplateResponse>(template);
    }

    public async Task<FormTemplateResponse> UpdateAsync(string id, UpdateFormTemplateRequest request)
    {
        var template = await _formTemplateRepository.GetAsync(id);
        if (template == null)
            throw new Exception("Plantilla no encontrada");

        template.Name = string.IsNullOrWhiteSpace(request.Name)
            ? template.Name
            : request.Name;

        template.Description = string.IsNullOrWhiteSpace(request.Description)
            ? template.Description
            : request.Description;

        await _formTemplateRepository.UpdateAsync(id, template);

        return _mapper.Map<FormTemplateResponse>(template);
    }

    public async Task<FormTemplateResponse> UpdateQuestionsAsync(string templateId, List<UpdateFormQuestionsRequest> questions)
    {
        var template = await _formTemplateRepository.GetAsync(templateId);
        if (template == null)
            throw new Exception("Plantilla no encontrada");

        foreach (var request in questions)
        {
            if (string.IsNullOrEmpty(request.Id))
                throw new Exception("El Id de la pregunta es obligatorio para editar");

            var questionId = ObjectId.Parse(request.Id);

            var existingQuestion = template.Questions
                .FirstOrDefault(q => q.Id == questionId);

            if (existingQuestion == null)
                throw new Exception($"Pregunta con Id {request.Id} no encontrada");

            // PATCH LOGIC
            if (!string.IsNullOrWhiteSpace(request.QuestionKey))
                existingQuestion.QuestionKey = request.QuestionKey;

            if (!string.IsNullOrWhiteSpace(request.QuestionText))
                existingQuestion.QuestionText = request.QuestionText;

            if (request.AnswerType.HasValue)
                existingQuestion.AnswerType = request.AnswerType.Value;
        }

        await _formTemplateRepository.UpdateAsync(templateId, template);

        return _mapper.Map<FormTemplateResponse>(template);
    }

    public async Task<bool> ActivateAsync(string id)
    {
        var template = await _formTemplateRepository.GetAsync(id);
        if (template == null)
            throw new Exception("Plantilla no encontrada");

        if (template.IsActive)
            return false;

        template.IsActive = true;
        await _formTemplateRepository.UpdateAsync(id, template);

        return true;
    }

    public async Task<bool> DeactivateAsync(string id)
    {
        var template = await _formTemplateRepository.GetAsync(id);
        if (template == null)
            throw new Exception("Plantilla no encontrada");

        if (!template.IsActive)
            return false;

        template.IsActive = false;
        await _formTemplateRepository.UpdateAsync(id, template);

        return true;
    }
}