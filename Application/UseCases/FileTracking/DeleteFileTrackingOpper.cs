using Application.DTOs.FileTracking;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.FileTracking
{
    public class DeleteFileTrackingOpper
    {
        private readonly IFileTrackingRepository _repository;
        private readonly IValidator<FileTrackingOpperDeleteDto> _validator;
        private readonly IMapper _mapper;

        public DeleteFileTrackingOpper(IFileTrackingRepository repository, IValidator<FileTrackingOpperDeleteDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(FileTrackingOpperDeleteDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();

                throw new AppValidationException(errores);
            }

            var entity = _mapper.Map<Core.Entities.FileTracking>(dto);
            await _repository.DeleteFileOpporAsync(entity.LinkToken, entity.OpporToken);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Archivo eliminado exitosamente.",
            };
        }
    }
}
