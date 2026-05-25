using Application.DTOs.Exercises;
using Application.DTOs.Hiring;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Hiring
{
    public class GetAllHiring
    {
        private readonly IHiringRepository _repository;
        private readonly IMapper _mapper;

        public GetAllHiring(IHiringRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PagedResult<HiringResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize, long? workerId, long? usersId)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize, workerId, usersId);

            return _mapper.Map<PagedResult<HiringResponseDto>>(entities);
        }
    }
}
