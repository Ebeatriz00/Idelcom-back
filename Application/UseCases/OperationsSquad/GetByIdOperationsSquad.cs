using Application.DTOs.OperationsSquad;
using AutoMapper;
using Core.Interfaces.OperationsSquad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.OperationsSquad
{
    public class GetByIdOperationsSquad(IOperationsSquadRepository repository, IMapper mapper)
    {
        private readonly IOperationsSquadRepository _repository = repository;
        private readonly IMapper _mapper = mapper;

        public async Task<OperationsSquadResponseDto?> ExecuteAsync(long squadId)
        {
            var entity = await _repository.GetByIdAsync(squadId);
            return _mapper.Map<OperationsSquadResponseDto?>(entity);
        }
    }       
}
