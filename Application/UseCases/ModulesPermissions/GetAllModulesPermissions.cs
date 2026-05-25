using Application.DTOs.ModulePermission;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;  

namespace Application.UseCases.ModulePermission
{
        public class GetAllModulesPermissions
        {
            private readonly IModulesPermissionsRepository _repository;
            private readonly IMapper _mapper;
            public GetAllModulesPermissions(IModulesPermissionsRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }
            public async Task<PagedResult<ModulesPermissionsResponseDto>> ExecuteAsync(int businessId, int page, int pageSize, string? search = null)
            {
                var entities = await _repository.GetAllAsync(businessId, page, pageSize, search);
                return _mapper.Map<PagedResult<ModulesPermissionsResponseDto>>(entities);
            }
        }
}


