using Application.DTOs.Operations.OperationsWorkOrderProgress;
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
        IValidator<OperationsWorkOrderProgressCreateDto> validator)
    {
        private readonly IOperationsWorkOrderProgressRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<OperationsWorkOrderProgressCreateDto> _validator = validator;

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
            return await _repository.CreateAsync(entity, userId, businessId);
        }
    }
}
