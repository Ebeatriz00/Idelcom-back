using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Core.Interfaces.Services;
using Core.Options;
using Core.Results.ApiPeru;
using Infrastructure.Exceptions;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.ApiPeru
{
    public class ApiPeruRucLookupService : IApiPeruRucLookupService
    {
        private readonly HttpClient _httpClient;
        private readonly ApiPeruOptions _options;

        public ApiPeruRucLookupService(HttpClient httpClient, IOptions<ApiPeruOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<ApiPeruRucLookupResult> GetByRucAsync(string ruc, CancellationToken cancellationToken = default)
        {
            var endpointPath = GetEndpointPath("Ruc");

            ValidateConfiguration(endpointPath);
            ConfigureApiKeyHeader();

            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    NormalizeEndpointPath(endpointPath),
                    new ApiPeruRucRequest { Ruc = ruc },
                    cancellationToken);

                if (!response.IsSuccessStatusCode)
                    throw new ExternalServiceException("Api Peru", $"StatusCode: {(int)response.StatusCode}");

                var providerResponse = await response.Content.ReadFromJsonAsync<ApiPeruRucResponse>(cancellationToken);

                if (providerResponse?.Success != true || providerResponse.Data == null)
                    throw new ExternalServiceException("Api Peru", "La respuesta del proveedor no contiene datos válidos.");

                return new ApiPeruRucLookupResult
                {
                    Direccion = providerResponse.Data.Direccion ?? string.Empty,
                    DireccionCompleta = providerResponse.Data.DireccionCompleta ?? string.Empty,
                    Ruc = providerResponse.Data.Ruc ?? string.Empty,
                    NombreORazonSocial = providerResponse.Data.NombreORazonSocial ?? string.Empty,
                    Estado = providerResponse.Data.Estado ?? string.Empty,
                    Condicion = providerResponse.Data.Condicion ?? string.Empty,
                    Departamento = providerResponse.Data.Departamento ?? string.Empty,
                    Provincia = providerResponse.Data.Provincia ?? string.Empty,
                    Distrito = providerResponse.Data.Distrito ?? string.Empty,
                    UbigeoSunat = providerResponse.Data.UbigeoSunat ?? string.Empty,
                    Ubigeo = providerResponse.Data.Ubigeo ?? Array.Empty<string>(),
                    EsAgenteDeRetencion = providerResponse.Data.EsAgenteDeRetencion ?? string.Empty,
                    EsAgenteDePercepcion = providerResponse.Data.EsAgenteDePercepcion ?? string.Empty,
                    EsAgenteDePercepcionCombustible = providerResponse.Data.EsAgenteDePercepcionCombustible ?? string.Empty,
                    EsBuenContribuyente = providerResponse.Data.EsBuenContribuyente ?? string.Empty
                };
            }
            catch (ExternalServiceException)
            {
                throw;
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new ExternalServiceException("Api Peru", "Timeout al consultar el proveedor.");
            }
            catch (Exception ex)
            {
                throw new ExternalServiceException("Api Peru", ex.Message);
            }
        }

        private string GetEndpointPath(string endpointName)
        {
            if (_options.Endpoints.TryGetValue(endpointName, out var endpointPath) &&
                !string.IsNullOrWhiteSpace(endpointPath))
            {
                return endpointPath;
            }

            return _options.EndpointPath;
        }

        private static string NormalizeEndpointPath(string endpointPath)
        {
            return endpointPath.TrimStart('/');
        }

        private void ValidateConfiguration(string endpointPath)
        {
            if (string.IsNullOrWhiteSpace(_options.BaseUrl) ||
                string.IsNullOrWhiteSpace(endpointPath) ||
                string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new ExternalServiceException("Api Peru", "Configuración ApiPeru incompleta.");
            }
        }

        private void ConfigureApiKeyHeader()
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKeyHeaderName))
                return;

            var headerValue = string.IsNullOrWhiteSpace(_options.ApiKeyHeaderValuePrefix)
                ? _options.ApiKey
                : $"{_options.ApiKeyHeaderValuePrefix} {_options.ApiKey}";

            _httpClient.DefaultRequestHeaders.Remove(_options.ApiKeyHeaderName);
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(_options.ApiKeyHeaderName, headerValue);

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private sealed class ApiPeruRucRequest
        {
            [JsonPropertyName("ruc")]
            public string Ruc { get; set; } = string.Empty;
        }

        private sealed class ApiPeruRucResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("data")]
            public ApiPeruRucData? Data { get; set; }
        }

        private sealed class ApiPeruRucData
        {
            [JsonPropertyName("direccion")]
            public string? Direccion { get; set; }

            [JsonPropertyName("direccion_completa")]
            public string? DireccionCompleta { get; set; }

            [JsonPropertyName("ruc")]
            public string? Ruc { get; set; }

            [JsonPropertyName("nombre_o_razon_social")]
            public string? NombreORazonSocial { get; set; }

            [JsonPropertyName("estado")]
            public string? Estado { get; set; }

            [JsonPropertyName("condicion")]
            public string? Condicion { get; set; }

            [JsonPropertyName("departamento")]
            public string? Departamento { get; set; }

            [JsonPropertyName("provincia")]
            public string? Provincia { get; set; }

            [JsonPropertyName("distrito")]
            public string? Distrito { get; set; }

            [JsonPropertyName("ubigeo_sunat")]
            public string? UbigeoSunat { get; set; }

            [JsonPropertyName("ubigeo")]
            public string[]? Ubigeo { get; set; }

            [JsonPropertyName("es_agente_de_retencion")]
            public string? EsAgenteDeRetencion { get; set; }

            [JsonPropertyName("es_agente_de_percepcion")]
            public string? EsAgenteDePercepcion { get; set; }

            [JsonPropertyName("es_agente_de_percepcion_combustible")]
            public string? EsAgenteDePercepcionCombustible { get; set; }

            [JsonPropertyName("es_buen_contribuyente")]
            public string? EsBuenContribuyente { get; set; }
        }
    }
}
