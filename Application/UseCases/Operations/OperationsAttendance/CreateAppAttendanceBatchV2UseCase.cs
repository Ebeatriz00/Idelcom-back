using Application.DTOs.Operations.OperationsAttendance;
using AutoMapper;
using Core.Interfaces.Operations;
using Core.Requests;
using FluentValidation;

namespace Application.UseCases.Operations.OperationsAttendance
{
    public class CreateAppAttendanceBatchV2UseCase(
        IOperationsAttendanceV2Repository repository,
        IMapper mapper,
        IValidator<AppAttendanceV2CreateDto> validator)
    {
        private readonly IOperationsAttendanceV2Repository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<AppAttendanceV2CreateDto> _validator = validator;

        public async Task<int> ExecuteAsync(AppAttendanceV2CreateDto dto, long businessId, long appUserId)
        {
            var validationResult = await _validator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var request = _mapper.Map<AppAttendanceBatchV2Request>(dto);
            request.BusinessId = businessId;
            request.UserId = appUserId;

            return await _repository.InsertBatchAsync(request, appUserId);
        }
    }
}
