using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.AssignmentEvaluationDTO;
using WorkTrace.Application.Enums;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Profiles;

public class AssignmentEvaluationProfile : Profile
{
    public AssignmentEvaluationProfile()
    {
        CreateMap<CreateAssignmentEvaluationRequest, AssignmentEvaluation>()
            .ForMember(dest => dest.AssignmentId,
                opt => opt.MapFrom(src => ObjectId.Parse(src.AssignmentId)))

            .ForMember(dest => dest.UserEvaluations,
                opt => opt.MapFrom(src => src.UserEvaluations))

            .ForMember(dest => dest.ClientComment,
                opt => opt.MapFrom(src => src.ClientComment))

            .ForMember(dest => dest.ClientSignature,
                opt => opt.MapFrom(src => src.ClientSignature))

            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UserEvaluationRequest, UserEvaluation>()
            .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => ObjectId.Parse(src.UserId)))

            .ForMember(dest => dest.Score,
                opt => opt.MapFrom(src =>
                    src.Answers
                        .Where(a => a.AnswerType == AnswerType.Numeric &&
                                    a.NumericValue.HasValue)
                        .Average(a => a.NumericValue!.Value)
                ))

            .ForMember(dest => dest.Answers,
                opt => opt.MapFrom(src => src.Answers));

        CreateMap<FormAnswerRequest, FormAnswers>()
            .ForMember(dest => dest.QuestionKey,
                opt => opt.MapFrom(src => src.QuestionKey))
            .ForMember(dest => dest.QuestionValue,
                opt => opt.MapFrom(src => src.QuestionValue))
            .ForMember(dest => dest.AnswerType,
                opt => opt.MapFrom(src => src.AnswerType))
            .ForMember(dest => dest.Answer,
                opt => opt.MapFrom(src => src.Answer))
            .ForMember(dest => dest.NumericValue,
                opt => opt.MapFrom(src => src.NumericValue));

        CreateMap<ClientSignatureRequest, ClientSignature>()
            .ForMember(dest => dest.SignatureBase64,
                opt => opt.MapFrom(src => src.SignatureBase64))
            .ForMember(dest => dest.SignedBy,
                opt => opt.MapFrom(src => src.SignedBy))
            .ForMember(dest => dest.SignedAt,
                opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}