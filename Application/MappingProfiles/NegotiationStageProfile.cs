using AutoMapper;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class NegotiationStageProfile : Profile
    {
        public NegotiationStageProfile() {
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
