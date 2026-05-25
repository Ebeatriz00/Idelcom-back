using Application.DTOs.Operations.OperationsWorkOrderActivity;
using AutoMapper;
using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsWorkOrderActivityProfile : Profile
    {
        public OperationsWorkOrderActivityProfile()
        {
            CreateMap<OperationsWorkOrderActivityCreateDto, OperationWorkOrderActivity>();
            CreateMap<OperationsWorkOrderActivityUpdateDto, OperationWorkOrderActivity>();
            CreateMap<OperationWorkOrderActivity, OperationsWorkOrderActivityResponseDto>();
            CreateMap<AppActivityWorkOrderProjection, AppActivitiesResponseDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
