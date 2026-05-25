using Application.DTOs.SomaDocumentType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaDocumentType
{
    public class GetByIdSsomaDocumentType
    {
        private readonly ISsomaDocumentTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetByIdSsomaDocumentType(ISsomaDocumentTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SsomaDocumentTypeResponseDto> ExecuteAsync(long ssomaDocumentTypeId)
        {
            var entity = await _repository.GetByIdAsync(ssomaDocumentTypeId);
            return _mapper.Map<SsomaDocumentTypeResponseDto>(entity);
        }
    }
}
