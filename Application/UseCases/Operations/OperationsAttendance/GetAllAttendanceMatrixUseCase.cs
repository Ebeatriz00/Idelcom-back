using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Operations;
using Core.Projections.Operations;
using FluentValidation;

namespace Application.UseCases.Operations.OperationsAttendance
{
    public class GetAllAttendanceMatrixUseCase(
        IOperationsAttendanceRepository repository,
        IMapper mapper,
        IValidator<AttendanceMatrixQueryDto> validator,
        IFileUrlBuilder fileUrlBuilder)
    {
        private readonly IOperationsAttendanceRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AttendanceMatrixQueryDto> _validator = validator;
        private readonly IFileUrlBuilder _fileUrlBuilder = fileUrlBuilder;

        public async Task<AttendanceMatrixResponseDto> ExecuteAsync(
            long businessId,
            AttendanceMatrixQueryDto query)
        {
            var validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var result = await _repository.GetMatrixAsync(
                businessId,
                query.StartDate!.Value,
                query.EndDate!.Value,
                query.OpporId,
                query.WorkOrderId,
                query.SquadId,
                query.Search,
                query.StatusId,
                query.Page,
                query.PageSize);

            var details = result.Details.ToList();
            result.Details = details;

            var response = _mapper.Map<AttendanceMatrixResponseDto>(result);
            SetPhotoUrls(response, details);

            return response;
        }

        private void SetPhotoUrls(
            AttendanceMatrixResponseDto response,
            IReadOnlyList<AttendanceMatrixDetailProjection> details)
        {
            for (var i = 0; i < response.Details.Count && i < details.Count; i++)
            {
                var source = details[i];
                var destination = response.Details[i];

                destination.CheckInGroupPhotoUid    = _fileUrlBuilder.BuildFileUrl(source.CheckInGroupPhotoUid);
                destination.CheckOutGroupPhotoUid   = _fileUrlBuilder.BuildFileUrl(source.CheckOutGroupPhotoUid);
                destination.CheckInPhotoUid         = _fileUrlBuilder.BuildFileUrl(source.CheckInPhotoUid);
                destination.CheckOutPhotoUid        = _fileUrlBuilder.BuildFileUrl(source.CheckOutPhotoUid);
            }
        }
    }
}
