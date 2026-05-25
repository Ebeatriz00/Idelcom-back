using Application.DTOs.PreSaleProyects;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.PreSaleProyects
{
   
        public class GetDetailPreSaleProyects
        {
            private readonly IPreSaleProyectsRepository _repository;
            private readonly IMapper _mapper;

            public GetDetailPreSaleProyects(IPreSaleProyectsRepository repository, IMapper mapper)
            {
                _repository = repository;
                _mapper = mapper;
            }

            public async Task<PreSaleProyectsDetailDto> ExecuteAsync(string linkToken, long businessId, CancellationToken ct = default)
            {
                var entity = await _repository.GetDetailAsync(linkToken, businessId, ct);
                return _mapper.Map<PreSaleProyectsDetailDto>(entity);
            }
        }
 }
