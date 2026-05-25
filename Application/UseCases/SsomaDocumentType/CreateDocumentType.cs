using Application.DTOs.SomaDocumentType;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Repositories;
using Org.BouncyCastle.Tsp;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaDocumentType
{
    public class CreateSsomaDocumentType
    {
        private readonly ISsomaDocumentTypeRepository _repository;
        private readonly IMapper _mapper;

        public CreateSsomaDocumentType(ISsomaDocumentTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(SsomaDocumentTypeCreateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.SsomaDocumentType>(dto);

            await _repository.AddAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Tipo de documento SSOMA creado exitosamente."
            };
        }
    }
}
