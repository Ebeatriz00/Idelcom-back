using Application.DTOs.Permissions;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Permissions
{
    public class GetByIdPermissions
    {
        private readonly IPermissionsRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdPermissions(IPermissionsRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PermissionsResponseDto> ExecuteAsync(long permissionId)
        {
            var entity = await _repository.GetByIdAsync(permissionId);
            return _mapper.Map<PermissionsResponseDto>(entity);
        }
    }

}
