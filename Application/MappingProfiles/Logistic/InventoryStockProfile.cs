using Application.DTOs.InventoryStock;
using AutoMapper;
using Core.Entities.Logistic;

namespace Application.MappingProfiles.Logistic
{
    public class InventoryStockProfile : Profile
    {
        public InventoryStockProfile()
        {
            CreateMap<InventoryStockCreateDto, InventoryStock>()
                .ForMember(dest => dest.LastEnteryDate, opt => opt.MapFrom(src => src.LastEntryDate));
        }
    }
}
