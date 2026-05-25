using Application.DTOs.SomaDocumentType;
using AutoMapper;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaDocumentType
{
    public class PatchSsomaDocumentType
    {
        private readonly ISsomaDocumentTypeRepository _repository;
        private readonly IMapper _mapper;

        public PatchSsomaDocumentType(ISsomaDocumentTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaDocumentTypeStatusToogleDto dto)
        {
            var updated = await _repository.PatchStatusAsync(
                dto.SsomaDocumentTypeId,
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del tipo de documento SSOMA actualizado correctamente."
                    : "No se pudo actualizar el estado del tipo de documento SSOMA."
            };
        }
    }
}
