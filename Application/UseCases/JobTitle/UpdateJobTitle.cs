using Application.DTOs.Area;
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


namespace Application.UseCases.JobTitle
{
    using Application.DTOs.JobTitle;
    using AutoMapper;
    using FluentValidation;
    using System.Linq;
    using System.Threading.Tasks;

    public class UpdateJobTitle
    {
        private readonly IJobTitleRepository _repository;
        private readonly IValidator<JobTitleUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateJobTitle(IJobTitleRepository repository, IValidator<JobTitleUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(JobTitleUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.JobTitleId))
            {
                throw new Exceptions.DuplicateEntryException("El cargo ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.JobTitle>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Cargo actualizado correctamente."
                    : "Error al actualizar el cargo."
            };
        }
    }
}