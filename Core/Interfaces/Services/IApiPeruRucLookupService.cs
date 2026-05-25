using Core.Results.ApiPeru;

namespace Core.Interfaces.Services
{
    public interface IApiPeruRucLookupService
    {
        Task<ApiPeruRucLookupResult> GetByRucAsync(string ruc, CancellationToken cancellationToken = default);
    }
}
