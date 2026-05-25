using Application.DTOs.Currency;
using Application.DTOs.FileTracking;
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


namespace Application.UseCases.FileTracking
{
    public class CreateFileTrackingOpper
    {
        private readonly IFileTrackingRepository _repository;
        private readonly IValidator<FileTrackingOpporCreateDto> _validator;
        private readonly IMapper _mapper;
        private readonly INotificationPush _push;
        public CreateFileTrackingOpper(IFileTrackingRepository repository, IValidator<FileTrackingOpporCreateDto> validator, IMapper mapper, INotificationPush push)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _push = push;
        }
        public async Task<GlobalResponse> ExecuteAsync(FileTrackingOpporCreateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                           .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                           .ToList();

                throw new AppValidationException(errores);
            }

            if (await _repository.ExistsOpporAsync(dto.FileUrl, dto.OpporToken, dto.BusinessId))
                throw new DuplicateEntryException("El archivo ya existe para esta oportunidad.");


            var entity = _mapper.Map<Core.Entities.FileTracking>(dto);
            await _repository.AddFileOpporAsync(entity);
            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "opportunities",
                "preSales",
                "hirings"
            );

            return new GlobalResponse
            {
                Status = 1,
                Message = "Archivo agregado exitosamente.",
            };
        }
    }
}
