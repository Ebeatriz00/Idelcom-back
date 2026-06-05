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
        IValidator<OperationsWorkOrderProgressSyncDto> validator,
        IOperationsWorkOrderActivityRepository activityRepository,
        IOperationsWorkOrderRepository workOrderRepository,
        IOperationsRepository operationsRepository,
        Application.UseCases.Operations.Operations.UpdateOperations updateOperationsUseCase)
    {
        private readonly IOperationsWorkOrderProgressRepository _progressRepository = progressRepository;
        private readonly IOperationsWorkOrderProgressPhotoRepository _photoRepository = photoRepository;
        private readonly IStorageService _storageService = storageService;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<OperationsWorkOrderProgressSyncDto> _validator = validator;
        private readonly IOperationsWorkOrderActivityRepository _activityRepository = activityRepository;
        private readonly IOperationsWorkOrderRepository _workOrderRepository = workOrderRepository;
        private readonly IOperationsRepository _operationsRepository = operationsRepository;
        private readonly Application.UseCases.Operations.Operations.UpdateOperations _updateOperationsUseCase = updateOperationsUseCase;

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
                    catch (Exception ex)
                    {
                        // En sincronización multimodal, un fallo en una foto no debe revertir el avance.
                        Console.WriteLine(ex.ToString());
                        continue;
                    }
                }
            }

            // 4. Lógica de Cambio de Estado a Ejecución (3) si es el primer avance
            if (response.Status == 1)
            {
                try
                {
                    var activity = await _activityRepository.GetByIdAsync(dto.ActivityId, businessId);
                    if (activity != null)
                    {
                        var workOrder = await _workOrderRepository.GetByIdAsync(activity.WorkOrderId, businessId);
                        if (workOrder != null)
                        {
                            var progressResult = await _progressRepository.GetAllAsync(businessId, null, 1, 1, null, null, workOrder.OperationsId);
                            if (progressResult.Total > 0)
                            {
                                var operation = await _operationsRepository.GetByIdAsync(workOrder.OperationsId);
                                if (operation != null && operation.OperationsStatusId == 2)
                                {
                                    var updateDto = new Application.DTOs.Operations.Operations.OperationsUpdateDto
                                    {
                                        OperationsId = operation.OperationsId,
                                        OpporId = operation.OpporId,
                                        QualitySupervisorId = operation.QualitySupervisorId,
                                        ProjectManagerId = operation.ProjectManagerId,
                                        RequeredSsoma = operation.RequeredSsoma,
                                        PlannedStartDate = operation.PlannedStartDate,
                                        ActualStartDate = operation.ActualStartDate,
                                        PlannedEndDate = operation.PlannedEndDate,
                                        ActualEndDate = operation.ActualEndDate,
                                        OperationsStatusId = 3,
                                        Status = operation.Status
                                    };

                                    await _updateOperationsUseCase.ExecuteAsync(updateDto, userId, businessId);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error actualizando estado a ejecución en Sincronización: {ex.Message}");
                }
            }

            return response;
        }
    }
}
