using Application.DTOs.PaymentType;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PaymentType
{
    public class GetByIdPaymentType
    {
        private readonly IPaymentTypeRepository _repository;
        private readonly IMapper _mapper;
        public GetByIdPaymentType(IPaymentTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PaymentTypeResponseDto> ExecuteAsync(long paymentTypeId)
        {
            var entities = await _repository.GetByIdAsync(paymentTypeId);
            return _mapper.Map<PaymentTypeResponseDto>(entities);
        }
    }
}
