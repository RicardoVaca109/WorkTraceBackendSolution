using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.TakenRequirementDTO;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Profiles;

public class TakenRequirementProfile : Profile
{
    public TakenRequirementProfile()
    {
        CreateMap<CreateTakenRequirementRequest, TakenRequirement>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.User,
                opt => opt.MapFrom(src => ObjectId.Parse(src.UserId)))
            .ForMember(dest => dest.Client,
                opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.ClientId)
                        ? (ObjectId?)null
                        : ObjectId.Parse(src.ClientId)))
            .ForMember(dest => dest.Date, opt => opt.Ignore());

        CreateMap<UpdateTakenRequirementRequest, TakenRequirement>()
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Date, opt => opt.Ignore())
            .ForMember(dest => dest.Client,
                opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.ClientId)
                        ? (ObjectId?)null
                        : ObjectId.Parse(src.ClientId)));

        CreateMap<TakenRequirement, TakenRequirementInformationResponse>()
            .ForMember(dest => dest.UserId,
                opt => opt.MapFrom(src => src.User.ToString()))
            .ForMember(dest => dest.ClientId,
                opt => opt.MapFrom(src =>
                    src.Client.HasValue
                        ? src.Client.Value.ToString()
                        : null));
    }
}