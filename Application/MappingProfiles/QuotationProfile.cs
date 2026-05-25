using Application.DTOs.Quotation;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class QuotationProfile :Profile
    {
        public QuotationProfile()
        {
            CreateMap<SalesQuotationCreateDto, SalesQuotation>();

            CreateMap<SalesQuotationLinDto, SalesQuotationLin>().ReverseMap();
            CreateMap<SalesQuotationMarginDto, SalesQuotationMargin>().ReverseMap();
            CreateMap<SalesQuotationServCheckDto, SalesQuotationServCheck>().ReverseMap();
            CreateMap<SalesQuotationLinePlanDto, SalesQuotationLinePlan>().ReverseMap();
            CreateMap<SalesQuotationLinePlanLinDto, SalesQuotationLinePlanLin>().ReverseMap();
            CreateMap<SalesQuotationEgressDto, SalesQuotationEgress>().ReverseMap();
            CreateMap<SalesQuotationEgressLinDto, SalesQuotationEgressLin>().ReverseMap();

            //-- Response Dto
            CreateMap<SalesQuotation, SalesQuotationResponseDto>();
            CreateMap<SalesQuotation, SalesQuotationVerResponseDto>();
            CreateMap<SalesQuotation, SalesQuotationDetailDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>))
                .ForMember("Items", opt => opt.MapFrom("Items"));

        }
    }
}
