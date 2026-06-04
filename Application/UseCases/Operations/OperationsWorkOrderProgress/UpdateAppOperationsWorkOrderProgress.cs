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
    public class UpdateAppOperationsWorkOrderProgress(
        IOperationsWorkOrderProgressRepository repository,
        IOperationsWorkOrderProgressPhotoRepository photoRepository,
        IStorageService storageService,
        IMapper mapper,
        IValidator<OperationsWorkOrderProgressUpdateDto> validator)
    {
        private readonly IOperationsWorkOrderProgressRepository _repository = repository;
        private readonly IOperationsWorkOrderProgressPhotoRepository _photoRepository = photoRepository;
        private readonly IStorageService _storageService = storageService;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<OperationsWorkOrderProgressUpdateDto> _validator = validator;

        public async Task<BaseResponseId> ExecuteAsync(
            OperationsWorkOrderProgressUpdateDto dto,
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
            var response = await _repository.UpdateAsync(entity, userId, businessId,
                dto.AppRecordId);

            if (response.Status == 1 && dto.Photos != null && dto.Photos.Count > 0)
            {
                long progressId = response.Id ?? 0;
                string suggestedPath = FileStoragePaths.OperationsWorkOrderProgress;

                foreach (var file in dto.Photos)
                {
                    try
                    {
                        using var stream = file.OpenReadStream();
                        var fileGuid = await _storageService.UploadAsync(
                            stream,
                            file.FileName,
                            suggestedPath,
                            userId);

                        await _photoRepository.InsertPhotoAsync(progressId, fileGuid, userId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        continue;
                    }
                }
            }

            return response;
        }
    }
}
