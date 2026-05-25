using Application.DTOs.Operations.MovementStatus;
using Application.Exceptions;
using AutoMapper;
using Core.Interfaces.Operations;

namespace Application.UseCases.Operations.MovementStatus
{
    public class GetByIdMovementStatus(IMovementStatusRepository repository, IMapper mapper)
    {
        private readonly IMovementStatusRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<MovementStatusGetByIdDto?> ExacuteAsync(long movementStatusId, long businessId)
        {
            var result = await _repository.GetByIdAsync(movementStatusId, businessId);

            if (result == null)
                throw new NotFoundException("MovementStatus not found.", movementStatusId);

            return _mapper.Map<MovementStatusGetByIdDto>(result);
        }
    }
}
