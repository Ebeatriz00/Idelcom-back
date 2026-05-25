using Application.DTOs.OperationsTeamSsoma;
using AutoMapper;
using Core.Entities.Operations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsTeamSsomaProfile : Profile
    {
        public OperationsTeamSsomaProfile()
        {
            CreateMap<OperationsTeamSsomaCreateDto, OperationsTeamSsoma>();
            CreateMap<OperationsTeamSsomaUpdateDto, OperationsTeamSsoma>();
            CreateMap<ProcessSsomaAssignmentChangeDto, OperationsTeamSsoma>();
            CreateMap<OperationsTeamSsomaAssignmentDto, OperationsTeamSsomaAssignmentItem>();
            CreateMap<OperationsTeamSsomaAssignmentUpdateDto, OperationsTeamSsomaAssignmentItem>();
            CreateMap<OperationsTeamSsomaAssignmentItem, OperationsTeamSsomaAssignmentDto>();
            CreateMap<OperationsTeamSsomaListItemProjection, OperationsTeamSsomaListItemDto>();
            CreateMap<OperationsTeamSsomaDetailProjection, OperationsTeamSsomaGetByIdDto>()
                .ForMember(
                    dest => dest.ClientApprovalDate,
                    opt => opt.MapFrom(src =>
                        src.ClientApprovalDate.HasValue
                            ? DateOnly.FromDateTime(src.ClientApprovalDate.Value)
                            : (DateOnly?)null));
        }
    }
}
