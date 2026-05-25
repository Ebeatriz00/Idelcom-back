using Application.DTOs.ReasonRejection;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.ReasonRejection
{
    public class GetByIdReasonRejection
    {
        private readonly IReasonRejectionRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdReasonRejection(IReasonRejectionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ReasonRejectionResponseDto> ExecuteAsync(long ReasonRejectionId)
        {
            var entity = await _repository.GetByIdAsync(ReasonRejectionId);
            return _mapper.Map<ReasonRejectionResponseDto>(entity);
        }
    }
}
