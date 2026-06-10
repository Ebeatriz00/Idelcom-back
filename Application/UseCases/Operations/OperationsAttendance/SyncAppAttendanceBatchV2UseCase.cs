using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Operations;
using Core.Requests;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.UseCases.Operations.OperationsAttendance
{
    public class SyncAppAttendanceBatchV2UseCase(
        IOperationsAttendanceV2Repository repository,
        IStorageService storageService,
        IMapper mapper,
        IValidator<AppAttendanceV2SyncDto> validator)
    {
        private readonly IOperationsAttendanceV2Repository _repository = repository;
        private readonly IStorageService _storageService = storageService;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AppAttendanceV2SyncDto> _validator = validator;

        public async Task<int> ExecuteAsync(AppAttendanceV2SyncDto dto, long businessId, long appUserId)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            Guid? groupPhotoUid = null;
            if (dto.GroupPhoto != null)
            {
                using var stream = dto.GroupPhoto.OpenReadStream();
                groupPhotoUid = await _storageService.UploadAsync(
                    stream,
                    dto.GroupPhoto.FileName,
                    FileStoragePaths.OperationsAttendanceGroups,
                    appUserId);
            }

            var request = _mapper.Map<AppAttendanceBatchV2Request>(dto);
            request.BusinessId = businessId;
            request.UserId = appUserId;
            request.GroupPhotoUid = groupPhotoUid;

            for (int i = 0; i < dto.Details.Count; i++)
            {
                var detailDto = dto.Details[i];
                if (detailDto.PhotoFile != null)
                {
                    using var stream = detailDto.PhotoFile.OpenReadStream();
                    var photoUid = await _storageService.UploadAsync(
                        stream,
                        detailDto.PhotoFile.FileName,
                        FileStoragePaths.OperationsAttendanceWorkers,
                        appUserId);

                    request.Details[i].PhotoUid = photoUid;
                }
            }

            return await _repository.InsertBatchAsync(request, appUserId);
        }
    }
}
