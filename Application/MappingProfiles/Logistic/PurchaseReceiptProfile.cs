using Application.DTOs.PurchaseReceipt;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Projections.Logistic;

namespace Application.MappingProfiles.Logistic
{
    public class PurchaseReceiptProfile : Profile
    {
        public PurchaseReceiptProfile()
        {
            CreateMap<PurchaseReceiptCreateDto, PurchaseReceiptCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.MapFrom(src => NormalizeId(src.PurchaseOrderId)));

            CreateMap<PurchaseReceiptDetailCreateDto, PurchaseReceiptDetailCommand>()
                .ForMember(dest => dest.PurchaseOrderDetailId, opt => opt.MapFrom(src => NormalizeId(src.PurchaseOrderDetailId)))
                .ForMember(dest => dest.UomId, opt => opt.MapFrom(src => NormalizeId(src.UomId)));

            CreateMap<PurchaseReceiptRegularizeDto, PurchaseReceiptRegularizeCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<PurchaseReceiptListItem, PurchaseReceiptResponseDto>();
            CreateMap<PurchaseReceiptHeaderProjection, PurchaseReceiptResponseDto>();
            CreateMap<PurchaseReceiptDetailProjection, PurchaseReceiptDetailResponseDto>();

            CreateMap<PurchaseReceiptByIdProjection, PurchaseReceiptFullResponseDto>()
                .ForMember(dest => dest.Header, opt => opt.MapFrom(src => src.Header))
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

            CreateMap<PurchaseReceiptHeaderProjection, PurchaseReceipt>()
                .ForMember(dest => dest.ReceiptNumber, opt => opt.MapFrom(src => src.ReceiptNumber ?? string.Empty))
                .ForMember(dest => dest.RegularizedPurchaseOrderId, opt => opt.MapFrom(src => src.RegularizedPurchaseOrderId))
                .ForMember(dest => dest.RegularizedDate, opt => opt.Ignore())
                .ForMember(dest => dest.RegularizedUser, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());

            CreateMap<PurchaseReceiptDetailProjection, PurchaseReceiptDetail>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());
        }

        private static long? NormalizeId(long? value)
        {
            return value.HasValue && value.Value > 0 ? value.Value : null;
        }
    }
}
