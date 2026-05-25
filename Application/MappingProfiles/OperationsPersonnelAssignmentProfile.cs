using Application.DTOs.OperationsPersonnelAssignment;
using AutoMapper;
using Core.Entities.OperationsPersonnelAssignment;
using Core.Entities.paginations;
using Core.Projections.OperationsPersonnelAssignment;

namespace Application.MappingProfiles
{
    public class OperationsPersonnelAssignmentProfile : Profile
    {
        public OperationsPersonnelAssignmentProfile()
        {
            CreateMap<OperationsPersonnelAssignmentCreateDto, OperationPersonnelAssignment>();
            CreateMap<OperationsPersonnelAssignmentUpdateDto, OperationPersonnelAssignment>();

            CreateMap<OperationPersonnelAssignment, OperationsPersonnelAssignmentResponseDto>(); 
            CreateMap<OperationPersonnelAssignmentProjection, OperationsPersonnelAssignmentResponseDto>();
        }
    }
}
