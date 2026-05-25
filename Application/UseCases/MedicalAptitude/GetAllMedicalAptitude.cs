using Application.DTOs.MedicalAptitude;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.MedicalAptitude
{
    public class GetAllMedicalAptitude
    {
        private readonly IMedicalAptitudeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllMedicalAptitude(IMedicalAptitudeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<MedicalAptitudeResponseDto>> ExecuteAsync(
            long businessId,
            string? search,
            int page,
            int pageSize,
            long? usersBy)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, usersBy);

            return _mapper.Map<PagedResult<MedicalAptitudeResponseDto>>(entities);
        }
    }
}
