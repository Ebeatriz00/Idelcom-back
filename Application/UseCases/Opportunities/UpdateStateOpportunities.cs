using Application.DTOs.Opportunities;
using Application.Exceptions;
using Application.Services.InterfacesServices;
using Application.Services.RealTime;
using Application.UseCases.Quotation;
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
using System.Transactions;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.Opportunities
{
    public class UpdateStateOpportunities
    {
        private readonly IOpportunitiesRepository _repository;
        private readonly IFileTrackingRepository _repositoryFile;
        private readonly IValidator<OpportunitiesStateUpdateDto> _validator;
        private readonly IMapper _mapper;

        private readonly ISalesQuotationExcelParserServices _excelParser;
        private readonly CreateSalesQuotation _createQuotation;

        private readonly INotificationPush _push;

        public UpdateStateOpportunities(IOpportunitiesRepository repository, IValidator<OpportunitiesStateUpdateDto> validator, IMapper mapper, ISalesQuotationExcelParserServices excelParser, CreateSalesQuotation createQuotation, IFileTrackingRepository repositoryFile, INotificationPush push)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _excelParser = excelParser;
            _createQuotation = createQuotation;
            _repositoryFile = repositoryFile;
            _push = push;
        }
        public async Task<GlobalResponse> ExecuteAsync(OpportunitiesStateUpdateDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();
                throw new AppValidationException(errores);
            }

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                TransactionScopeAsyncFlowOption.Enabled
            );

            

            if (dto.StateOpporId == 4 && dto.StateOpporGenId == 1)
            {

                if (await _repositoryFile.ExistsOpporAsync(dto.FileUrl, dto.LinkToken, dto.BusinessId))
                    throw new DuplicateEntryException("La cotización ya fue subida, sube una nueva con un nombre diferente.");


                if (dto.ExcelFile == null || dto.ExcelFile.Length == 0)
                    throw new QuotationFileFoundException("Debe adjuntar el archivo de cotización.");

                using var stream = dto.ExcelFile.OpenReadStream();
                var quotationDto = _excelParser.Parse(stream);

                quotationDto.BusinessId = dto.BusinessId;
                quotationDto.UsersBy = dto.UsersBy;
                quotationDto.OpporId = long.Parse(dto.LinkToken);

                await _createQuotation.ExecuteAsync(quotationDto);
            }

            var entity = _mapper.Map<Core.Entities.Opportunities>(dto);
            var updated = await _repository.UpdateStateAsync(entity);

            if (!updated)
                throw new Exception("No se pudo actualizar el estado de la oportunidad."); 

            scope.Complete();

            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                dto.BusinessId,
                "preSales",
                "hirings",
                "opportunities"
            );



            return new GlobalResponse
            {
                Status = 1,
                Message = "Estado de la oportunidad actualizado correctamente."
            };
        }
    }
}
