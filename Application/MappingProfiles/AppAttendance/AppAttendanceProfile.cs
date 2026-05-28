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
                .ForMember(dest => dest.ProjectConfigs, opt => opt.Ignore());

            CreateMap<AppAttendanceProjectConfigProjection, AppAttendanceProjectConfigDto>()
                .ForMember(dest => dest.EntryTime, opt => opt.MapFrom(src => src.EntryTime.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.DepartureTime, opt => opt.MapFrom(src => src.DepartureTime.ToString(@"hh\:mm\:ss")))
                .ForMember(dest => dest.BeforeOfficialTime, opt => opt.MapFrom(src => src.BeforeOfficialTime.ToString(@"hh\:mm\:ss")));

            CreateMap<AppAttendanceWorkOrderProjection, AppAttendanceWorkOrderDto>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.HasValue ? src.EndDate.Value.ToString("yyyy-MM-dd") : null));

            CreateMap<AppAttendanceSquadProjection, AppAttendanceSquadDto>()
                .ForMember(dest => dest.OperationsProjectConfigId, opt => opt.MapFrom(src => src.OperationsProjectConfigId))
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