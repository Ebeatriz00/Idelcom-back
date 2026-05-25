using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using AutoMapper;
using Core.Entities.Operations.Core.Entities;

namespace Application.MappingProfiles.Operations
{

    public class OperationsWorkOrderResponsibleProfile : Profile
    {
        public OperationsWorkOrderResponsibleProfile()
        {
            CreateMap<OperationsWorkOrderResponsibleCreateDto, OperationWorkOrderResponsible>();
            CreateMap<OperationsWorkOrderResponsibleUpdateDto, OperationWorkOrderResponsible>();

            CreateMap<OperationWorkOrderResponsible, OperationsWorkOrderResponsibleResponseDto>();
            CreateMap<OperationWorkOrderResponsible, OperationsWorkOrderResponsibleByIdDto>();
        }
    }

}
