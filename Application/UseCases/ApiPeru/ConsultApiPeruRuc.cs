using Application.DTOs.ApiPeru;
using Core.Interfaces.Services;
using FluentValidation;
using SharedKernel;
using AppValidationException = Application.Exceptions.ValidationException;

namespace Application.UseCases.ApiPeru
{
    public class ConsultApiPeruRuc
    {
        private readonly IApiPeruRucLookupService _apiPeruRucLookupService;
        private readonly IValidator<ApiPeruRucLookupRequestDto> _validator;

        public ConsultApiPeruRuc(IApiPeruRucLookupService apiPeruRucLookupService, IValidator<ApiPeruRucLookupRequestDto> validator)
        {
            _apiPeruRucLookupService = apiPeruRucLookupService;
            _validator = validator;
        }

        public async Task<GlobalResponse<ApiPeruRucLookupResponseDto>> ExecuteAsync(ApiPeruRucLookupRequestDto dto, CancellationToken cancellationToken = default)
        {
            var validation = await _validator.ValidateAsync(dto, cancellationToken);

            if (!validation.IsValid)
            {
                var errors = validation.Errors
                    .Select(e => new GlobalErrorDetail(e.ErrorCode, e.ErrorMessage))
                    .ToList();

                throw new AppValidationException(errors);
            }

            var result = await _apiPeruRucLookupService.GetByRucAsync(dto.Ruc, cancellationToken);

            return new GlobalResponse<ApiPeruRucLookupResponseDto>
            {
                Status = 1,
                Message = "Consulta RUC realizada exitosamente.",
                Data = new ApiPeruRucLookupResponseDto
                {
                    Direccion = result.Direccion,
                    DireccionCompleta = result.DireccionCompleta,
                    Ruc = result.Ruc,
                    NombreORazonSocial = result.NombreORazonSocial,
                    Estado = result.Estado,
                    Condicion = result.Condicion,
                    Departamento = result.Departamento,
                    Provincia = result.Provincia,
                    Distrito = result.Distrito,
                    UbigeoSunat = result.UbigeoSunat,
                    Ubigeo = result.Ubigeo,
                    EsAgenteDeRetencion = result.EsAgenteDeRetencion,
                    EsAgenteDePercepcion = result.EsAgenteDePercepcion,
                    EsAgenteDePercepcionCombustible = result.EsAgenteDePercepcionCombustible,
                    EsBuenContribuyente = result.EsBuenContribuyente
                }
            };
        }
    }
}
