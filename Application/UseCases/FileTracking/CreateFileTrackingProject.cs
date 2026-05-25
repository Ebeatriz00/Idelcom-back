using Application.DTOs.FileTracking;
using Application.Exceptions;
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
    public class CreateFileTrackingProject
    {
        private readonly IFileTrackingRepository _repository;
        private readonly IValidator<FileTrackingProjectCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateFileTrackingProject(IFileTrackingRepository repository, IValidator<FileTrackingProjectCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(FileTrackingProjectCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();

                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsProjectAsync(dto.FileUrl, dto.ProjectToken, dto.BusinessId))
                throw new DuplicateEntryException("El archivo ya existe para este proyecto.");


            var entity = _mapper.Map<Core.Entities.FileTracking>(dto);
            await _repository.AddFileProjectAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Archivo agregado exitosamente.",
            };
        }
    }
}
