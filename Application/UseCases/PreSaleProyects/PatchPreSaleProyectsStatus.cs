using Application.DTOs.ContactsCrm;
using Application.DTOs.PreSaleProyects;
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
    public class PatchPreSaleProyectsStatus
    {
        private readonly IPreSaleProyectsRepository _repository;
        private readonly IValidator<PreSaleProyectsStatusToggleDto> _validator;
        private readonly IMapper _mapper;

        public PatchPreSaleProyectsStatus(
            IPreSaleProyectsRepository repository,
            IValidator<PreSaleProyectsStatusToggleDto> validator,
            IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(PreSaleProyectsStatusToggleDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                        .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                        .ToList();
                throw new AppValidationException(errores);
            }

            var updated = await _repository.PatchStatusAsync(
                dto.LinkToken,
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del proyecto actualizado correctamente."
                    : "No se pudo actualizar el estado del proyecto."
            };
        }
    }
}
