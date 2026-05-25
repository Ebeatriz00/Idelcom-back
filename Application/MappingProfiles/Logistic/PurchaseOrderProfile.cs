using Application.DTOs.PurchaseOrder;
using AutoMapper;
using Core.Commands.Logistic;
using Core.Entities.Logistic;
using Core.Projections.Logistic;

namespace Application.MappingProfiles.Logistic
{
    public class PurchaseOrderProfile : Profile
    {
        public PurchaseOrderProfile()
        {
            CreateMap<PurchaseOrderCreateDto, PurchaseOrderCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore());

            CreateMap<PurchaseOrderDetailCreateDto, PurchaseOrderDetailCommand>()
                .ForMember(dest => dest.PurchaseOrderDetailId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            CreateMap<PurchaseOrderUpdateDto, PurchaseOrderCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<PurchaseOrderDetailUpdateDto, PurchaseOrderDetailCommand>();

            CreateMap<PurchaseOrderAttachInvoiceDto, PurchaseOrderAttachInvoiceCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<PurchaseOrderCreateFromInvoiceDto, PurchaseOrderCreateFromInvoiceCommand>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<PurchaseOrderListItem, PurchaseOrderListResponse>();
            CreateMap<PurchaseOrderHeaderProjection, PurchaseOrderGetByIdResponse>()
                .ForMember(dest => dest.Details, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore());
            CreateMap<PurchaseOrderDetailProjection, PurchaseOrderDetailResponse>();
            CreateMap<PurchaseOrderInvoiceProjection, PurchaseOrderInvoiceResponse>();

            CreateMap<PurchaseOrderHeaderProjection, PurchaseOrder>()
                .ForMember(dest => dest.PurchaseOrderNumber, opt => opt.MapFrom(src => src.PurchaseOrderNumber ?? string.Empty))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "1"))
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());

            CreateMap<PurchaseOrderDetailProjection, PurchaseOrderDetail>()
                .ForMember(dest => dest.BusinessId, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseOrderId, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());

            CreateMap<PurchaseOrderInvoiceProjection, PurchaseOrderInvoice>()
                .ForMember(dest => dest.Observation, opt => opt.MapFrom(src => src.Observation ?? string.Empty))
                .ForMember(dest => dest.CreateUser, opt => opt.Ignore())
                .ForMember(dest => dest.CreateDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateUser, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore());
        }
    }
}
