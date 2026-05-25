using Application.DTOs.Operations.OperationsWorkOrder;
using AutoMapper;
using Core.Entities.Operations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsWorkOrderProfile : Profile
    {
        public OperationsWorkOrderProfile()
        {
            CreateMap<OperationsWorkOrderCreateDto, OperationWorkOrder>();
            CreateMap<OperationsWorkOrderUpdateDto, OperationWorkOrder>();
            CreateMap<OperationsWorkOrderDeleteDto, OperationWorkOrder>();

            CreateMap<OperationWorkOrder, OperationsWorkOrderListDto>();
            CreateMap<OperationWorkOrder, OperationsWorkOrderByIdDto>();
        }
    }
}
