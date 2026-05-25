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
    public class PatchWorkerStatus
    {
        private readonly IWorkerStatusRepository _repository;
        private readonly IMapper _mapper;

        public PatchWorkerStatus(IWorkerStatusRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(WorkerStatusStatusToogleDto dto)
        {
            var updated = await _repository.PatchStatusAsync(
                dto.WorkerStatusId,
                dto.Status,
                dto.UsersBy,
                dto.BusinessId
            );

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
