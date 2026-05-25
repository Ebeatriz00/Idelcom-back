using Application.DTOs.OperationsTeamSsoma;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.OperationsTeamSsoma
{
    public class GetListByProcessIdOperationsTeamSsoma(
        IOperationsTeamSsomaRepository repository,
        IMapper mapper)
    {
        private readonly IOperationsTeamSsomaRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<IEnumerable<OperationsTeamSsomaListItemDto>> ExecuteAsync(long businessId, long ssomaProcessId)
        {
            var projection = await _repository.GetListByProcessIdAsync(businessId, ssomaProcessId);
            
            // Si la proyección es nula, devolvemos lista vacía para evitar "source fue null"
            if (projection == null)
            {
                return new List<OperationsTeamSsomaListItemDto>();
            }

            return _mapper.Map<IEnumerable<OperationsTeamSsomaListItemDto>>(projection);
        }
    }
}
