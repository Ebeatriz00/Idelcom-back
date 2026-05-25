using Application.DTOs.InventoryCount;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Projections.Logistic;

namespace Application.MappingProfiles.Logistic
{
    public class InventoryCountProfile : Profile
    {
        public InventoryCountProfile()
        {
            CreateMap<InventoryCountCreateDto, InventoryCountCreateCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CountDate, opt => opt.MapFrom(src => src.CountDate ?? DateTime.Today));

            CreateMap<InventoryCountDetailUpdateDto, InventoryCountDetailUpdateCommand>();

            CreateMap<InventoryCountUpdateDetailsDto, InventoryCountUpdateDetailsCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<InventoryCountListItem, InventoryCountListResponseDto>();
            CreateMap<InventoryCountHeaderProjection, InventoryCountListResponseDto>();
            CreateMap<InventoryCountDetailProjection, InventoryCountDetailResponseDto>();

            CreateMap<InventoryCountHeaderProjection, InventoryCount>()
                .ForMember(dest => dest.CountNumber, opt => opt.MapFrom(src => src.CountNumber ?? string.Empty))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());

            CreateMap<InventoryCountDetailProjection, InventoryCountDetail>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());
        }
    }
}
