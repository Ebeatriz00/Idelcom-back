using Application.DTOs.Operations.MeasurementUnit;
using AutoMapper;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class MeasurementUnitProfile : Profile
    {
        public MeasurementUnitProfile()
        {
            CreateMap<MeasurementUnitSelectItem, MeasurementUnitSelectDto>();

            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
