using Application.DTOs.Permissions;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Permissions
{
    public class GetAllPermissions
    {
            private readonly IPermissionsRepository _repository;
            private readonly IMapper _mapper;

            public GetAllPermissions(IPermissionsRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<PagedResult<PermissionsResponseDto>> ExecuteAsync(int businessId, int page, int pageSize)
            {
                var entities = await _repository.GetAllAsync(businessId, page, pageSize );
                return _mapper.Map<PagedResult<PermissionsResponseDto>>(entities);
            }
    }
}
