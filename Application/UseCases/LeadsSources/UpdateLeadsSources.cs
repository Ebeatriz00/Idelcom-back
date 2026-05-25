using Application.DTOs.LeadsSources;
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

namespace Application.UseCases.LeadsSources
{
    public class UpdateLeadsSources
    {
        private readonly ILeadsSourcesRepository _repository;
        private readonly IValidator<LeadsSourcesUpdateDto> _validator;
        private readonly IMapper _mapper;
        public UpdateLeadsSources(ILeadsSourcesRepository repository, IValidator<LeadsSourcesUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(LeadsSourcesUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.LeadsSourcesId))
                throw new DuplicateEntryException("La fuente de lead ya existe para este negocio.");


            var entity = _mapper.Map<Core.Entities.LeadsSources>(dto);
            var updated = await _repository.UpdateAsync(entity);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Fuente de lead actualizada correctamente." : "No se pudo actualizar la fuente de lead."
            };
        }
    }
}
