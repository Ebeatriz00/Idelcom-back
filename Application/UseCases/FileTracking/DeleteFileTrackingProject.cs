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
    public class DeleteFileTrackingProject
    {
        private readonly IFileTrackingRepository _repository;
        private readonly IValidator<FileTrackingProjectDeleteDto> _validator;
        private readonly IMapper _mapper;

        public DeleteFileTrackingProject(IFileTrackingRepository repository, IValidator<FileTrackingProjectDeleteDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(FileTrackingProjectDeleteDto dto)
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
            await _repository.DeleteFileProjectAsync(entity.LinkToken, entity.ProjectToken);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Archivo eliminado exitosamente.",
            };
        }
    }
}
