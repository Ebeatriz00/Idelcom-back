using Application.DTOs.ReasonRejection;
using AutoMapper;
using Core.Entities.paginations;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ReasonRejection
{
    public class GetAllReasonRejection
    {
        private readonly IReasonRejectionRepository _repository;
        private readonly IMapper _mapper;
        public GetAllReasonRejection(IReasonRejectionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<ReasonRejectionResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<ReasonRejectionResponseDto>>(entities);
        }
    }
}
