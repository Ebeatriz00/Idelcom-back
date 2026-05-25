using Application.DTOs.Ssoma;
using Application.DTOs.SsomaAssignmanetType;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.SsomaAssignmanetType
{
    public class GetAllSsomaAssignmanetType
    {
        private readonly ISsomaAssignmanetTypeRepository _repository;
        private readonly IMapper _mapper;

        public GetAllSsomaAssignmanetType(ISsomaAssignmanetTypeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<PagedResult<SsomaAssignmanetTypeResponseDto>> ExecuteAsync(long businessId, string? search, int page, int pageSize)
        {
            var entities = await _repository.GetAllAsync(businessId, search, page, pageSize);
            return _mapper.Map<PagedResult<SsomaAssignmanetTypeResponseDto>>(entities);
        }
    }
}
