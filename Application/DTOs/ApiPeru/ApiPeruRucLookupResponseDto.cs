namespace Application.DTOs.ApiPeru
{
    public class ApiPeruRucLookupResponseDto
    {
        public string Direccion { get; set; } = string.Empty;
        public string DireccionCompleta { get; set; } = string.Empty;
        public string Ruc { get; set; } = string.Empty;
        public string NombreORazonSocial { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Condicion { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string Provincia { get; set; } = string.Empty;
        public string Distrito { get; set; } = string.Empty;
        public string UbigeoSunat { get; set; } = string.Empty;
        public IReadOnlyList<string> Ubigeo { get; set; } = Array.Empty<string>();
        public string EsAgenteDeRetencion { get; set; } = string.Empty;
        public string EsAgenteDePercepcion { get; set; } = string.Empty;
        public string EsAgenteDePercepcionCombustible { get; set; } = string.Empty;
        public string EsBuenContribuyente { get; set; } = string.Empty;
    }
}
