using Application.DTOs.LeadsStatus;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.UseCases.LeadsStatus
{
    public class PatchLeadsStatus
    {
        private readonly ILeadsStatusRepository _repository;
        private readonly IValidator<LeadsStatusStatusToggleDto> _validator;
        private readonly IMapper _mapper;

        public PatchLeadsStatus(ILeadsStatusRepository repository, IValidator<LeadsStatusStatusToggleDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(LeadsStatusStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new Application.Exceptions.ValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(dto.LeadsStatusId, dto.Status, dto.UsersBy, dto.BusinessId);
            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated ? "Estado de lead actualizado correctamente." : "No se pudo actualizar el estado de la calificación de lead."
            };
        }
    }
}
