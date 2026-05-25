using Application.DTOs.DashboardPreSales;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class DashboardPreSalesProfile : Profile
    {
        public DashboardPreSalesProfile()
        {
            CreateMap<DashboardPreSalesQuotation, DashboardPreSalesQuotationDto>();
            CreateMap<DashboardPreSalesState, DashboardPreSalesStateDto>();
            CreateMap<DashboardPreSalesCombined, DashboardPreSalesCombinedDto>();
            CreateMap<DashboardPreSalesByEngineer, DashboardPreSalesByEngineerDto>();
            CreateMap<DashboardPreSalesMatriz, DashboardPreSalesMatrizDto>();
            CreateMap<DashboardPreSalesCollaborator, DashboardPreSalesCollaboratorDto>();
            CreateMap<DashboardPreSalesIntegrators, DashboardPreSalesIntegratorsDto>();
            CreateMap<DashboardPreSaleByEngineerDetails, DashboardPreSaleByEngineerDetailsDto>();
            CreateMap<DashboardPreSalesIntegratorsDetails, DashboardPreSalesIntegratorsDetailsDto>();
            CreateMap<DashboardPreSalesCollaboratorDetails, DashboardPreSalesCollaboratorDetailsDto>();
            CreateMap<DashboardPreSalesByCategory, DashboardPreSalesByCategoryDto>();
        }
    }
}
