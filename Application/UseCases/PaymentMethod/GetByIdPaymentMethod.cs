using Application.DTOs.PaymentMethod;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PaymentMethod
{
    public class GetPaymentMethodById
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly IMapper _mapper;

        public GetPaymentMethodById(IPaymentMethodRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaymentMethodByIdDto> ExecuteAsync(long paymentMethodId)
        {
            var entity = await _repository.GetByIdAsync(paymentMethodId);
            return _mapper.Map<PaymentMethodByIdDto>(entity);
        }
    }
}
