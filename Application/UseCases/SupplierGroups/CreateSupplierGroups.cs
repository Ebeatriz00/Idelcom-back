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
    public class CreateSupplierGroups
    {
        private readonly ISupplierGroupsRepository _repository;
        private readonly IValidator<SupplierGroupsCreateDto> _validator;
        private readonly IMapper _mapper;

        public CreateSupplierGroups(ISupplierGroupsRepository repository, IValidator<SupplierGroupsCreateDto> validator, IMapper mapper)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SupplierGroupsCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                                .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                                .ToList();

                throw new AppValidationException(errores);
            }

            var yaExiste = await _repository.ExistsAsync(dto.Description, dto.BusinessId);
            if (yaExiste)
                throw new DuplicateEntryException("El grupo de proveedores ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.SupplierGroups>(dto);
            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Grupo de proveedores creado exitosamente.",
            };
        }
    }
}
