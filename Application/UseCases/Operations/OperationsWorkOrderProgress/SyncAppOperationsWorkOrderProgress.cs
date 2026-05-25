using Application.DTOs.Operations.OperationsWorkOrderProgress;
using AutoMapper;
using Core.Entities.Operations;
using Core.Interfaces;
using Core.Interfaces.Operations;
using FluentValidation;
using SharedKernel;
using SharedKernel.Constants;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.OperationsWorkOrderProgress
{
    public class SyncAppOperationsWorkOrderProgress(
        IOperationsWorkOrderProgressRepository progressRepository,
        IOperationsWorkOrderProgressPhotoRepository photoRepository,
        IStorageService storageService,
        IMapper mapper,
        IValidator<OperationsWorkOrderProgressSyncDto> validator)
    {
        private readonly IOperationsWorkOrderProgressRepository _progressRepository = progressRepository;
        private readonly IOperationsWorkOrderProgressPhotoRepository _photoRepository = photoRepository;
        private readonly IStorageService _storageService = storageService;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<OperationsWorkOrderProgressSyncDto> _validator = validator;

        public async Task<BaseResponseId> ExecuteAsync(
            OperationsWorkOrderProgressSyncDto dto,
            long userId,
            long businessId)
        {
            // 0. Validación
            var validation = await _validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errors);
            }

            // 1. Mapeo y registro principal (con Barrera de Idempotencia en SP)
            var entity = _mapper.Map<OperationWorkOrderProgress>(dto);

            var (response, isDuplicate) = await _progressRepository.CreateV2Async(
                entity,
                userId,
                businessId,
                dto.AppRecordId);

            // 2. Si es duplicado, ignoramos el procesamiento de archivos y retornamos éxito
            if (isDuplicate)
            {
                return response;
            }

            // 3. Si es un registro nuevo exitoso, procesamos las evidencias fotográficas
            if (response.Status == 1 && dto.Photos != null && dto.Photos.Count > 0)
            {
                long progressId = response.Id ?? 0;
                string suggestedPath = FileStoragePaths.OperationsWorkOrderProgress;

                foreach (var file in dto.Photos)
                {
                    try
                    {
                        // Subir al almacenamiento físico y registro central de archivos
                        using var stream = file.OpenReadStream();
                        var fileGuid = await _storageService.UploadAsync(
                            stream,
                            file.FileName,
                            suggestedPath,
                            userId);

                        // Vincular la foto con el avance de obra en la tabla puente
                        await _photoRepository.InsertPhotoAsync(progressId, fileGuid, userId);
                    }
                    catch (Exception)
                    {
                        // En sincronización multimodal, un fallo en una foto no debe revertir el avance.
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
