using Application.DTOs.WorkerStatus;
using AutoMapper;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.WorkerStatus
{
    public class UpdateWorkerStatus
    {
        private readonly IWorkerStatusRepository _repository;
        private readonly IMapper _mapper;

        public UpdateWorkerStatus(IWorkerStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(WorkerStatusUpdateDto dto)
        {
            var entity = _mapper.Map<Core.Entities.WorkerStatus>(dto);

            var updated = await _repository.UpdateAsync(entity);

            return new GlobalResponse
            {
                Status = updated ? 1 : 0,
                Message = updated
                    ? "Estado del trabajador actualizado correctamente."
                    : "No se pudo actualizar el estado del trabajador."
            };
        }
    }
}
