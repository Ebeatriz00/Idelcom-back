using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Operations;
using Core.Requests;
using FluentValidation;
using SharedKernel.Constants;

namespace Application.UseCases.Operations.OperationsAttendance
{
    public class SyncAppAttendanceBatchUseCase(
        IOperationsAttendanceRepository repository,
        IStorageService storageService,
        IMapper mapper,
        IValidator<AppAttendanceSyncDto> validator)
    {
        private readonly IOperationsAttendanceRepository _repository = repository;
        private readonly IStorageService _storageService = storageService;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AppAttendanceSyncDto> _validator = validator;

        public async Task<int> ExecuteAsync(AppAttendanceSyncDto dto, long businessId, long appUserId)
        {
            // 1. Validar
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // 2. Procesar Foto Grupal (Opcional)
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

            // 3. Mapear a objeto de Core
            var request = _mapper.Map<AppAttendanceBatchRequest>(dto);
            request.BusinessId = businessId;
            request.UserId = appUserId;
            request.GroupPhotoUid = groupPhotoUid;

            // 4. Procesar Fotos Individuales (Opcionales)
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

            // 5. Ejecutar en repositorio
            return await _repository.InsertBatchAsync(request, appUserId);
        }
    }
}
