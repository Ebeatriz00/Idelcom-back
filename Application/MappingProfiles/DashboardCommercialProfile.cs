using Application.DTOs.Dashboard;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class DashboardCommercialProfile : Profile
    {
        public DashboardCommercialProfile()
        {
            CreateMap<StateOpportunityMetric, StateOpportunityMetricDto>();
            CreateMap<ClientMetric, ClientMetricDto>();
            CreateMap<QuarterMetric, QuarterMetricDto>();
            CreateMap<CombinedMetric, CombinedMetricDto>();
            CreateMap<DashboardCommercialProbability, DashboardCommercialProbabilityDto>();
            CreateMap<DashboardCommercialEvolution, DashboardCommercialEvolutionDto>();
            CreateMap<DashboardCommercialClosing, DashboardCommercialClosingDto>();
            CreateMap<DashboardCommercialClient, DashboardCommercialClientDto>();
            CreateMap<DashboardCommercialTotals, DashboardCommercialTotalsDto>();
        }
    }
}
