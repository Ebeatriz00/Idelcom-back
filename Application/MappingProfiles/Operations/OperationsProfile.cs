using Application.DTOs.Operations.Operations;
using AutoMapper;
using Core.Entities.Operations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsProfile : Profile
    {
        public OperationsProfile()
        {
            CreateMap<Operation, OperationsResponseDto>();
            CreateMap<OperationsListItemProjection, OperationsResponseDto>();
            CreateMap<OperationsListItemProjection, Operation>();
            CreateMap<OperationsCreateDto, Operation>();
            CreateMap<OperationsUpdateDto, Operation>();
            CreateMap<OperationsDeleteDto, Operation>();
        }
    }
}
