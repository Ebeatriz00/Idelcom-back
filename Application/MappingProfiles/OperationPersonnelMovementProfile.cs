using Application.DTOs.OperationsPersonnelMovement;
using AutoMapper;
using Core.Entities.OperationsPersonnelMovement;
using Core.Entities.paginations;
using Core.Projections.OperationsPersonnelMovement;

namespace Application.MappingProfiles
{
    public class OperationPersonnelMovementProfile : Profile
    {
        public OperationPersonnelMovementProfile()
        {
            CreateMap<OperationPersonnelMovementCreateDto, OperationPersonnelMovement>();
            CreateMap<OperationPersonnelMovement, OperationPersonnelMovementResponseDto>();
            CreateMap<OperationsPersonnelMovementProjection, OperationPersonnelMovementResponseDto>();
            
            CreateMap<PagedResult<OperationPersonnelMovement>, PagedResult<OperationPersonnelMovementResponseDto>>();
            CreateMap<PagedResult<OperationsPersonnelMovementProjection>, PagedResult<OperationPersonnelMovementResponseDto>>();
        }
    }
}
