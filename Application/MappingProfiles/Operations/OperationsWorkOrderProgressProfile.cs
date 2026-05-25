using Application.DTOs.Operations.OperationsWorkOrderProgress;
using AutoMapper;
using Core.Entities.Operations;
using Core.Entities.paginations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsWorkOrderProgressProfile : Profile
    {
        public OperationsWorkOrderProgressProfile()
        {
            CreateMap<OperationsWorkOrderProgressCreateDto, OperationWorkOrderProgress>();
            CreateMap<OperationsWorkOrderProgressSyncDto, OperationWorkOrderProgress>();
            CreateMap<OperationWorkOrderProgress, OperationsWorkOrderProgressResponseDto>();
            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
        }
    }
}
