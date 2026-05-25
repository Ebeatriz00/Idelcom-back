using Application.DTOs.ModulePermission;
using Application.DTOs.ModulesPermissions;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ModulePermission
{
        public class GetByIdModulesPermissions
        {
            private readonly IModulesPermissionsRepository _repository;
            private readonly IMapper _mapper;

            public GetByIdModulesPermissions(IModulesPermissionsRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<ModulesPermissionsByIdDto> ExecuteAsync(long modulesPermissionsId)
            {
                var entity = await _repository.GetByIdAsync(modulesPermissionsId);
                return _mapper.Map<ModulesPermissionsByIdDto>(entity);
            }
        }
}
