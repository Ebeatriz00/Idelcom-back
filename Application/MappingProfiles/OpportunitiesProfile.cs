using Application.DTOs.Opportunities;
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
    public class OpportunitiesProfile : Profile
    {
        public OpportunitiesProfile()
        {
            CreateMap<OpportunitiesCreateDto, Opportunities>();
            CreateMap<OpportunitiesUpdateDto, Opportunities>();
            CreateMap<OpportunitiesClientsUpdateDto, Opportunities>();
            CreateMap<OpportunitiesStateUpdateDto, Opportunities>();
            CreateMap<Opportunities, OpportunitiesResponseDto>();
            CreateMap<OpportunitiesUploadNewVerDto, Opportunities>();

            CreateMap<OpportFiletrackingDto, OpportFiletracking>();

            CreateMap<Opportunities, OpportunitiesGetByIdDto>();
            CreateMap<Opportunities, OpportunitiesStateGetByIdDto>();
            CreateMap<Opportunities, OpportunitiesClientsGetByIdDto>();
            CreateMap<OpportunitiesStateGetByIdDto, Opportunities>();

            CreateMap<OpportTask, OpportTaskDto>();
            CreateMap<OpportProjectTeam, OpportProjectTeamDto>();
            CreateMap<OpportActivity, OpportActivityDto>();
            CreateMap<OpportFiletracking, OpportFiletrackingDto>();
            CreateMap<Opportunities, OpportunitiesDetailDto>();
            CreateMap<HistoryOpportunityChanges, HistoryOpportunityChangesDto>();

            CreateMap<Opportunities, OpportunitiesSelectDto>();
            CreateMap<DeliverablesOpporDto, DeliverablesOppor>();
            CreateMap<DeliverablesOppor, DeliverablesOpporDto>();

            CreateMap<ObservationOppor, ObservationOpporDto>();
            CreateMap<ObservationOpporDto, ObservationOppor>();

            CreateMap<DeliverablesOppor, DeliverablesOpporByIdDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
