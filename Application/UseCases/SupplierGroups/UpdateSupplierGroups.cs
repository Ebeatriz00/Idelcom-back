using Application.DTOs.SupplierGroups;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Logistic;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;


namespace Application.UseCases.SupplierGroups
{
    public class UpdateSupplierGroups
    {
        private readonly ISupplierGroupsRepository _repository;
        private readonly IValidator<SupplierGroupsUpdateDto> _validator;
        private readonly IMapper _mapper;

        public UpdateSupplierGroups(ISupplierGroupsRepository repository, IValidator<SupplierGroupsUpdateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SupplierGroupsUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                    .ToList();

                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsAsync(dto.Description, dto.BusinessId, dto.SupplierGroupsId))
            {
                throw new DuplicateEntryException("El grupo de proveedores ya existe para este negocio.");
            }

            var entity = _mapper.Map<Core.Entities.SupplierGroups>(dto);
            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                ? "Grupo de proveedores actualizado correctamente."
                : "No se pudo actualizar el grupo de proveedores."
            };
        }
    }
}
