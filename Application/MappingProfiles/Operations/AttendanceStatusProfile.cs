using Application.DTOs.Operations.AttendanceStatus;
using AutoMapper;
using Core.Entities.Operations;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class AttendanceStatusProfile : Profile
    {
        public AttendanceStatusProfile()
        {
            CreateMap<AttendanceStatusSelectItem, AttendanceStatusGetSelectDto>();
            CreateMap<AttendanceStatus, AttendanceStatusGetByIdDto>();
        }
    }

}
