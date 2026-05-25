using Application.DTOs.Operations.OperationsWorkOrderStatus;
using AutoMapper;
using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsWorkOrderStatusProfile : Profile
    {
        public OperationsWorkOrderStatusProfile()
        {
            CreateMap<OperationsWorkOrderStatusSelectItem, OperationsWorkOrderStatusSelectDto>();
            CreateMap<OperationWorkOrderStatus, OperationsWorkOrderStatusGetByIdDto>();

            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }

}
