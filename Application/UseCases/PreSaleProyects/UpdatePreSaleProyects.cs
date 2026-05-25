using Application.DTOs.PreSaleProyects;
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


namespace Application.UseCases.PreSaleProyects
{
    public class UpdatePreSaleProyects
    {
        private readonly IPreSaleProyectsRepository _repository;
        private readonly IValidator<PreSaleProyectsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdatePreSaleProyects(
            IPreSaleProyectsRepository repository,
            IValidator<PreSaleProyectsUpdateDto> validator,
            IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PreSaleProyectsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.LinkToken))
            {
                throw new DuplicateEntryException("El proyecto ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.PreSaleProyects>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Proyecto actualizado correctamente."
                    : "Error al actualizar el proyecto."
            };
        }
    }
}
