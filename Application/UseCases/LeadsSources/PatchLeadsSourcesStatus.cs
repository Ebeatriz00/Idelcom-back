
using Application.DTOs.LeadsSources;
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
    public class PatchLeadsSourcesStatus
    {
        private readonly ILeadsSourcesRepository _repository;
        private readonly IValidator<LeadsSourcesStatusToggleDto> _validator;
        private readonly IMapper _mapper;
        public PatchLeadsSourcesStatus(ILeadsSourcesRepository repository, IValidator<LeadsSourcesStatusToggleDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }
        public async Task<GlobalResponse> ExecuteAsync(LeadsSourcesStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.LeadsSourcesId, dto.Status, dto.UsersBy, dto.BusinessId);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Fuente de lead actualizado correctamente." : "No se pudo actualizar el fuente de lead."
            };
        }
    }
}
