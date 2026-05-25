using Application.DTOs.WarehousesMovement;
using AutoMapper;
using Core.Entities.Logistic;
using Core.Projections.Logistic;

namespace Application.MappingProfiles.Logistic
{
    public class WarehousesMovementProfile : Profile
    {
        public WarehousesMovementProfile()
        {
            CreateMap<WarehousesMovementCreateDto, WarehousesMovement>()
                .ForMember(dest => dest.WarehouseDestinationId, opt => opt.MapFrom(src => src.WarehouseDestinationId ?? 0))
                .ForMember(dest => dest.SuppliersId, opt => opt.MapFrom(src => src.SuppliersId ?? 0))
                .ForMember(dest => dest.ClientsId, opt => opt.MapFrom(src => src.ClientsId ?? 0))
                .ForMember(dest => dest.SubTotal, opt => opt.Ignore())
                .ForMember(dest => dest.IgvPercent, opt => opt.Ignore())
                .ForMember(dest => dest.Igv, opt => opt.Ignore())
                .ForMember(dest => dest.Total, opt => opt.Ignore());

            CreateMap<WarehousesMovementDetailCreateDto, WarehouseMovementDetail>();

            CreateMap<WarehouseMovementListItem, WarehouseMovementListDto>();
            CreateMap<WarehouseMovementDetailProjection, WarehouseMovementDetailDto>();
            CreateMap<InventoryStockAvailableProjection, InventoryStockAvailableDto>();
            CreateMap<WarehouseMovementHeaderProjection, WarehouseMovementByIdDto>();
        }
    }
}
