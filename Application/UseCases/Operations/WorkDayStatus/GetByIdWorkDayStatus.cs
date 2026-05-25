using Application.DTOs.Operations.WorkDayStatus;
using AutoMapper;
using Core.Interfaces.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Operations.WorkDayStatus
{
    public class GetByIdWorkDayStatus(IWorkDayStatusRepository repository, IMapper mapper)
    {
        private readonly IWorkDayStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;
        public async Task<WorkDayStatusGetByIdDto?> ExecuteAsync(long WorkdayStatusId, long businessId)
        {
            var result = await _repository.GetByIdAsync(WorkdayStatusId, businessId);
            if (result == null)
                return null;
            return _mapper.Map<WorkDayStatusGetByIdDto>(result);
        }
    }
}
