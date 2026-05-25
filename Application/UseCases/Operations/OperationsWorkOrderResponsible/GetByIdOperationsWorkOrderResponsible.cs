using Application.DTOs.Operations.OperationsWorkOrderResponsible;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.OperationsWorkOrderResponsible
{
    public class GetByIdOperationsWorkOrderResponsible(IOperationsWorkOrderResponsibleRepository repository, IMapper mapper)
    {
        private readonly IOperationsWorkOrderResponsibleRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsWorkOrderResponsibleByIdDto?> ExecuteAsync(
            long workOrderResponsibleId,
            long businessId)
        {
            var result = await _repository.GetByIdAsync(workOrderResponsibleId, businessId);

            if (result == null)
                throw new NotFoundException("Operations Work Order Responsible", workOrderResponsibleId);

            return _mapper.Map<OperationsWorkOrderResponsibleByIdDto>(result);
        }
    }

}
