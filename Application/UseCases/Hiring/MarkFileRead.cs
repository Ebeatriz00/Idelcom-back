using Application.DTOs.Hiring;
using AutoMapper;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Hiring
{
    public class MarkFilesRead
    {
        private readonly IHiringRepository _repository;
        private readonly IMapper _mapper;

        public MarkFilesRead(IHiringRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GlobalResponse> ExecuteAsync(MarkFileReadDto model)
        {
            var entity = _mapper.Map<Core.Entities.FileTracking>(model);

            await _repository.MarkFilesReadAsync(entity);

            return new GlobalResponse
            {
                Status = 1,
                Message = "Notificaciones actualizadas."
            };
        }
    }
}
