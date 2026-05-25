using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Interfaces.Operations;
using Core.Requests;
using FluentValidation;

namespace Application.UseCases.Operations.OperationsAttendance
{
    public class CreateAppAttendanceBatchUseCase(
        IOperationsAttendanceRepository repository,
        IMapper mapper,
        IValidator<AppAttendanceCreateDto> validator)
    {
        private readonly IOperationsAttendanceRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AppAttendanceCreateDto> _validator = validator;

        public async Task<int> ExecuteAsync(AppAttendanceCreateDto dto, long businessId, long appUserId)
        {
            // Validar
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // Mapear a objeto de Core
            var request = _mapper.Map<AppAttendanceBatchRequest>(dto);
            request.BusinessId = businessId;
            request.UserId = appUserId;

            // Ejecutar en repositorio
            return await _repository.InsertBatchAsync(request, appUserId);
        }
    }
}
