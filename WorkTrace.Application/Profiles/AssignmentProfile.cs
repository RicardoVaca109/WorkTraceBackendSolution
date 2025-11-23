using AutoMapper;
using MongoDB.Bson;
using WorkTrace.Application.DTOs.AssignmentDTO.Management;
using WorkTrace.Data.Models;

namespace WorkTrace.Application.Profiles;

public class AssignmentProfile : Profile
{
    public AssignmentProfile()
    {
        CreateMap<CreateAssignmentRequest, Assignment>()
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => ObjectId.Parse(src.Client)))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => ObjectId.Parse(src.Service)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ObjectId.Parse(src.Status)))
            .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => ObjectId.Parse(src.CreatedByUser)))
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users.Select(ObjectId.Parse).ToList()))
            .ForMember(dest => dest.DestinationLocation, opt => opt.Ignore());

        CreateMap<UpdateAssignmentMobileRequest, Assignment>();

        CreateMap<UpdateAssignmentWebRequest, Assignment>()
            .ForMember(dest => dest.Client, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Client)))
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => ObjectId.Parse(src.Client)))
            .ForMember(dest => dest.Service, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Service)))
            .ForMember(dest => dest.Service, opt => opt.MapFrom(src => ObjectId.Parse(src.Service)))
            .ForMember(dest => dest.Status, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Status)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ObjectId.Parse(src.Status)))
            .ForMember(dest => dest.CreatedByUser, opt => opt.Condition(src => !string.IsNullOrEmpty(src.CreatedByUser)))
            .ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => ObjectId.Parse(src.CreatedByUser)))
            .ForMember(dest => dest.Users, opt => opt.Condition(src => src.Users != null && src.Users.Any()))
            .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users.Select(ObjectId.Parse).ToList()))
            .ForMember(dest => dest.AssignedDate, opt => opt.Condition(src => src.AssignedDate.HasValue))
            .ForMember(dest => dest.AssignedDate, opt => opt.MapFrom(src => src.AssignedDate.Value))
            .ForMember(dest => dest.Address, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Address)))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));

        CreateMap<Assignment, AssignmentResponse>()
            .ForMember(dest => dest.AssignedDate, opt => opt.MapFrom(src => src.AssignedDate.ToString("dd-MM-yyyy")))
            .ForMember(dest => dest.AssignedTime, opt => opt.MapFrom(src => src.AssignedDate.ToString("HH:mm")));
    }
}