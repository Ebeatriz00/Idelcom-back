using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Projections.AppAttendance;
using Core.Requests;

namespace Application.MappingProfiles.AppAttendance
{
    public class AppAttendanceProfile : Profile
    {
        public AppAttendanceProfile()
        {
            // 1. Mapeos para la consulta (Proyecciones -> DTOs)
            CreateMap<AppAttendanceOperationProjection, AppAttendanceOperationDto>()
                .ForMember(dest => dest.ProjectConfig, opt => opt.MapFrom(src => src));

            CreateMap<AppAttendanceOperationProjection, AppAttendanceProjectConfigDto>()
                .ForMember(dest => dest.EntryTime, opt => opt.MapFrom(src => src.EntryTime.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.BeforeOfficialTime, opt => opt.MapFrom(src => src.BeforeOfficialTime.ToString(@"hh\:mm\:ss")));

            CreateMap<AppAttendanceSquadProjection, AppAttendanceSquadDto>()
                .ForMember(dest => dest.TechLeader, opt => opt.MapFrom(src => new AppAttendanceTechLeaderDto
                {
                    WorkerId = src.TechLeaderId,
                    FullName = $"{src.TechLeaderName} {src.TechLeaderLastName}".Trim(),
                    Document = src.TechLeaderDocument
                }));

            CreateMap<AppAttendanceWorkerProjection, AppAttendanceWorkerDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.WorkerName} {src.WorkerLastName}".Trim()))
                .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.WorkerDocument));

            CreateMap<AppAttendanceSessionProjection, AppAttendanceSessionInfoDto>();

            CreateMap<AppAttendanceDetailProjection, AppAttendanceDetailDto>()
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId));

            CreateMap<AppAttendanceStatusProjection, AppAttendanceStatusDto>();

            // 2. Mapeos para el registro (DTOs -> Core Requests)
            CreateMap<AppAttendanceCreateDto, AppAttendanceBatchRequest>();
            CreateMap<AppAttendanceBatchDetailDto, AppAttendanceBatchRequest.AppAttendanceBatchDetailRequest>();

            CreateMap<AppAttendanceSyncDto, AppAttendanceBatchRequest>()
                .ForMember(dest => dest.GroupPhotoUid, opt => opt.Ignore())
                .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details));

            CreateMap<AppAttendanceBatchDetailSyncDto, AppAttendanceBatchRequest.AppAttendanceBatchDetailRequest>()
                .ForMember(dest => dest.PhotoUid, opt => opt.Ignore());
        }
    }
}