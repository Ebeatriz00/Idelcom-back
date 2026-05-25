using Application.DTOs.Operations.OperationsStatus;
using AutoMapper;
using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsStatusProfile : Profile
    {
        public OperationsStatusProfile()
        {
            CreateMap<OperationsStatusSelectItem, OperationsStatusSelectDto>();
            CreateMap<OperationsStatus, OperationsStatusGeByIdDto>();
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
