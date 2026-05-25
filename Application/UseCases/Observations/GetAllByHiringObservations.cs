using Application.DTOs.Observations;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Observations
{
    public class GetAllByHiringObservations
    {
        private readonly IObservationsRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILinkTokenService _linkToken;

        public GetAllByHiringObservations(
            IObservationsRepository repository,
            IMapper mapper,
            ILinkTokenService linkToken)
        {
            _repository = repository;
            _mapper = mapper;
            _linkToken = linkToken;
        }

        public async Task<IEnumerable<ObservationsResponseDto>> ExecuteAsync(string opporToken)
        {
            long opporId = 0;

            if (!string.IsNullOrWhiteSpace(opporToken))
            {
                if (_linkToken.TryValidate(opporToken, out var entity, out var resourceId) && entity == "opportunity")
                {
                    opporId = Convert.ToInt64(resourceId);
                }
            }

            if (opporId == 0)
            {
                return new List<ObservationsResponseDto>();
            }

            var entities = await _repository.GetAllByHiringAsync(opporId);
            return _mapper.Map<IEnumerable<ObservationsResponseDto>>(entities);
        }
    }
}
