using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.FormTemplateDTO.Information;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Profiles;

public class FormTemplateProfile : Profile
{
    public FormTemplateProfile()
    {
        CreateMap<CreateFormTemplateRequest, FormTemplate>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<CreateFormQuestionRequest, FormQuestion>();

        // UPDATE TEMPLATE (metadata)
        CreateMap<UpdateFormTemplateRequest, FormTemplate>()
            .ForMember(dest => dest.Questions, opt => opt.Ignore());

        // UPDATE QUESTION
        CreateMap<UpdateFormQuestionsRequest, FormQuestion>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => string.IsNullOrEmpty(src.Id)
                    ? ObjectId.GenerateNewId()
                    : ObjectId.Parse(src.Id)));

        // RESPONSE
        CreateMap<FormTemplate, FormTemplateResponse>();
        CreateMap<FormQuestion, FormQuestionResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

    }
}