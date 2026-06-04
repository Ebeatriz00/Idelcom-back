using Core.Entities.MobileAppVersion;

namespace Core.Interfaces.MobileAppVersion
{
    public interface IMobileAppVersionConfigRepository
    {
        Task<MobileAppVersionConfig?> GetActiveAsync(
            string platform,
            string environment,
            CancellationToken cancellationToken = default);

        Task<MobileAppVersionConfig> UpsertActiveAsync(
            MobileAppVersionConfig config,
            CancellationToken cancellationToken = default);
    }
}
