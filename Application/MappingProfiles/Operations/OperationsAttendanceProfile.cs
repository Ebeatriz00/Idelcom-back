using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Projections.Operations;

namespace Application.MappingProfiles.Operations
{
    public class OperationsAttendanceProfile : Profile
    {
        public OperationsAttendanceProfile()
        {
            CreateMap<AttendanceMatrixProjectProjection, AttendanceMatrixProjectDto>();
            CreateMap<AttendanceMatrixWorkOrderProjection, AttendanceMatrixWorkOrderDto>();
            CreateMap<AttendanceMatrixSquadProjection, AttendanceMatrixSquadDto>();
            CreateMap<AttendanceMatrixDetailProjection, AttendanceMatrixDetailDto>();
            CreateMap<AttendanceMatrixResult, AttendanceMatrixResponseDto>();
        }
    }
}
