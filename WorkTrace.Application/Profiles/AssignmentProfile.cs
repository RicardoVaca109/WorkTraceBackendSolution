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
        CreateMap<Assignment, AssignmentResponse>();
    }
}