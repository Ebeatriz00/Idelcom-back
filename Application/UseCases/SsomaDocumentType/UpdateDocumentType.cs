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
    public class UpdateSsomaDocumentType
    {
        private readonly ISsomaDocumentTypeRepository _repository;
        private readonly IMapper _mapper;

        public UpdateSsomaDocumentType(ISsomaDocumentTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaDocumentTypeUpdateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.SsomaDocumentType>(dto);

            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Tipo de documento SSOMA actualizado correctamente."
                    : "No se pudo actualizar el tipo de documento SSOMA."
            };
        }
    }
}
