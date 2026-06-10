using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Projections.AppAttendance;
using Core.Requests;

namespace Application.MappingProfiles.AppAttendance
{
    public class AppAttendanceV2Profile : Profile
    {
        public AppAttendanceV2Profile()
        {
            // Proyecciones → DTOs
            CreateMap<AppAttendanceOperationProjection, AppAttendanceOperationV2Dto>()
                .ForMember(dest => dest.ProjectConfigs, opt => opt.Ignore())
                .ForMember(dest => dest.Sessions, opt => opt.Ignore())
                .ForMember(dest => dest.Workers, opt => opt.Ignore());

            CreateMap<AppAttendanceWorkerV2Projection, AppAttendanceWorkerV2Dto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.WorkerName} {src.WorkerLastName}".Trim()))
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.WorkerDocument))
                .ForMember(dest => dest.Attendance, opt => opt.Ignore());

            CreateMap<AppAttendanceSessionV2Projection, AppAttendanceSessionInfoDto>();

            // DTOs → Core Requests
            CreateMap<AppAttendanceV2CreateDto, AppAttendanceBatchV2Request>();
            CreateMap<AppAttendanceBatchV2DetailDto, AppAttendanceBatchV2Request.AppAttendanceBatchV2DetailRequest>();

            CreateMap<AppAttendanceV2SyncDto, AppAttendanceBatchV2Request>()
                .ForMember(dest => dest.GroupPhotoUid, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

            CreateMap<AppAttendanceBatchDetailV2SyncDto, AppAttendanceBatchV2Request.AppAttendanceBatchV2DetailRequest>()
                .ForMember(dest => dest.PhotoUid, opt => opt.Ignore());
        }
    }
}
