using Core.Entities.Security;
using Core.Interfaces.Security;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories.Security
{
    public class AppDeviceSessionRepository(IDapperHelper dapperHelper) : IAppDeviceSessionRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<long> CreateAsync(AppDeviceSession session)
        {
            var parameters = DapperParams.From(new
            {
                session.UserId,
                session.DeviceId,
                session.DeviceName,
                session.DeviceBrand,
                session.DeviceVersion,
                session.AppVersion,
                session.Jti,
            });

            return await _dapperHelper.ExecuteScalarAsync<long>("SP_WS_CREATE_APP_SESSION", parameters);
        }

        public async Task<bool> IsRevokedAsync(string jti)
        {
            var parameters = DapperParams.From(new { Jti = jti });

            return await _dapperHelper.ExecuteScalarAsync<bool>("SP_WS_CHECK_APP_SESSION_REVOKED", parameters);
        }

        public async Task<bool> RevokeByDeviceIdAsync(string deviceId)
        {
            var parameters = DapperParams.From(new { DeviceId = deviceId });

            return await _dapperHelper.ExecuteScalarAsync<bool>("SP_WS_REVOKE_ALL_DEVICE_APP_SESSIONS", parameters);
        }

        public async Task<bool> RevokeByJtiAsync(string jti)
        {
            var parameters = DapperParams.From(new { Jti = jti });

            return await _dapperHelper.ExecuteScalarAsync<bool>("SP_WS_REVOKE_APP_SESSION", parameters);
        }
    }
}
