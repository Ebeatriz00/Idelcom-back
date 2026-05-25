using Application.DTOs.JobTitle;
using AutoMapper;
using Core.Interfaces;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.JobTitle
{
    public class GetJobTitleById
    {
        private readonly IJobTitleRepository _repository;
        private readonly IMapper _mapper;

        public GetJobTitleById(IJobTitleRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<JobTitleByIdDto> ExecuteAsync(long jobTitleId)
        {
            var entity = await _repository.GetByIdAsync(jobTitleId);
            return _mapper.Map<JobTitleByIdDto>(entity);
        }
    }
}
