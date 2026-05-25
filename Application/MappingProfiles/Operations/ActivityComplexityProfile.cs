using Application.DTOs.Operations.ActivityComplexity;
using AutoMapper;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class ActivityComplexityProfile : Profile
    {
        public ActivityComplexityProfile()
        {
            CreateMap<ActivityComplexitySelectItem, ActivityComplexitySelectDto>();

            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
