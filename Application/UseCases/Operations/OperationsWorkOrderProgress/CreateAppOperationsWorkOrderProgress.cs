using Application.DTOs.Operations.Operations;
using Application.DTOs.Operations.OperationsWorkOrderProgress;
using Application.UseCases.Operations.Operations;
using AutoMapper;
using Core.Entities.Operations;
using Core.Interfaces.Operations;
using FluentValidation;
using SharedKernel;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Operations.OperationsWorkOrderProgress
{
    public class CreateAppOperationsWorkOrderProgress(
        IOperationsWorkOrderProgressRepository repository,
        IMapper mapper,
        IValidator<OperationsWorkOrderProgressCreateDto> validator,
        IOperationsWorkOrderActivityRepository activityRepository,
        IOperationsWorkOrderRepository workOrderRepository,
        IOperationsRepository operationsRepository,
        UpdateOperations updateOperationsUseCase)
    {
        private readonly IOperationsWorkOrderProgressRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<OperationsWorkOrderProgressCreateDto> _validator = validator;
        private readonly IOperationsWorkOrderActivityRepository _activityRepository = activityRepository;
        private readonly IOperationsWorkOrderRepository _workOrderRepository = workOrderRepository;
        private readonly IOperationsRepository _operationsRepository = operationsRepository;
        private readonly UpdateOperations _updateOperationsUseCase = updateOperationsUseCase;

        public async Task<BaseResponseId> ExecuteAsync(
            OperationsWorkOrderProgressCreateDto dto,
            long userId,
            long businessId)
        {
            var validation = await _validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errors);
            }

            var entity = _mapper.Map<OperationWorkOrderProgress>(dto);
            var result = await _repository.CreateAsync(entity, userId, businessId);

            if (result.Status == 1)
            {
                try
                {
                    var activity = await _activityRepository.GetByIdAsync(dto.ActivityId, businessId);
                    if (activity != null)
                    {
                        var workOrder = await _workOrderRepository.GetByIdAsync(activity.WorkOrderId, businessId);
                        if (workOrder != null)
                        {
                            var progressResult = await _repository.GetAllAsync(businessId, null, 1, 1, null, null, workOrder.OperationsId);
                            if (progressResult.Total > 0)
                            {
                                var operation = await _operationsRepository.GetByIdAsync(workOrder.OperationsId);
                                if (operation != null && operation.OperationsStatusId == 2)
                                {
                                    var updateDto = new OperationsUpdateDto
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
                    Console.WriteLine($"Error actualizando estado a ejecución: {ex.Message}");
                }
            }

            return result;
        }
    }
}
