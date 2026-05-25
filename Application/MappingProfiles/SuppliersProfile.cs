using Application.DTOs.Suppliers;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Entities.paginations;
using Core.Projections.Logistic;

namespace Application.MappingProfiles
{
    public class SuppliersProfile : Profile
    {
        public SuppliersProfile()
        {
            CreateMap<SuppliersCreateDto, Suppliers>();
            CreateMap<SuppliersUpdateDto, Suppliers>();

            CreateMap<SupplierItem, SuppliersResponseDto>();

            CreateMap<Suppliers, SuppliersResponseDto>();
            CreateMap<Suppliers, SuppliersByIdDto>();
            CreateMap<Suppliers, SuppliersSelectDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
