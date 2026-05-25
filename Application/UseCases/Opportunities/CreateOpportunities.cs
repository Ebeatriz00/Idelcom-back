using Application.DTOs.Opportunities;
using Application.Exceptions;
using Application.Services.RealTime;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using FluentValidation;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppValidationException = Application.Exceptions.ValidationException;



namespace Application.UseCases.Opportunities
{
    public class CreateOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IValidator<OpportunitiesCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly IEmailSender _email;
        private readonly IEmailTemplateRenderer _tpl;
        private readonly INotificationPush _push;

        public CreateOpportunities(IOpportunitiesRepository repository, IValidator<OpportunitiesCreateDto> validator, IMapper mapper, IEmailSender email, IEmailTemplateRenderer tpl, INotificationPush push)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _email = email;
            _tpl = tpl;
            _push = push;
        }
        public async Task<GlobalResponse> ExecuteAsync(OpportunitiesCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage)).ToList();
                throw new AppValidationException(errores);
            }
            if (await _repository.ExistsAsync(dto.OpporDesc, dto.BusinessId))
                throw new DuplicateEntryException("la oportunidad ya existe para este negocio.");

            var entity = _mapper.Map<Core.Entities.Opportunities>(dto);
            var dbRes = await _repository.AddAsync(entity);
            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "preSales",
                "hirings",
                "opportunities"
            );
            if (dbRes.Status != 1)
            {
                return dbRes;
            }

            return dbRes;
        }
    }
}
