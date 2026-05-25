using Application.DTOs.Operations.AssignmentStatus;
using AutoMapper;
using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class AssignmentStatusProfile : Profile
    {
        public AssignmentStatusProfile()
        {

            CreateMap<AssignmentStatusSelectItem, AssignmentStatusSelectDto>();
            CreateMap<AssignmentStatus, AssignmentStatusGetByIdDto>();

            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }

}
