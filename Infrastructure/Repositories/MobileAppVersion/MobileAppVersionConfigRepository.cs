using Core.Entities.MobileAppVersion;
using Core.Interfaces.MobileAppVersion;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.MobileAppVersion
{
    public class MobileAppVersionConfigRepository(IDapperHelper dapperHelper) : IMobileAppVersionConfigRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<MobileAppVersionConfig?> GetActiveAsync(
            string platform,
            string environment,
            CancellationToken cancellationToken = default)
        {
            var parameters = DapperParams.From(new
            {
                Platform = platform,
                Environment = environment
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<MobileAppVersionConfig>(
                "SP_WS_GET_MOBILE_APP_VERSION_CONFIG",
                parameters);
        }

        public async Task<MobileAppVersionConfig> UpsertActiveAsync(
            MobileAppVersionConfig config,
            CancellationToken cancellationToken = default)
        {
            var parameters = DapperParams.From(new
            {
                config.Platform,
                config.Environment,
                config.MinimumVersionName,
                config.MinimumBuildNumber,
                config.LatestVersionName,
                config.LatestBuildNumber,
                config.ForceUpdate,
                config.StoreUrl,
                config.Message
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<MobileAppVersionConfig>(
                "SP_WS_UPSERT_MOBILE_APP_VERSION_CONFIG",
                parameters)
                ?? throw new InvalidOperationException("No se pudo guardar la configuracion de version movil.");
        }
    }
}
