using Core.Entities.paginations;
using Core.Entities.Ssoma.Clinics;
using Core.Interfaces.Ssoma.Clinics;
using Core.Projections.Ssoma.Clinics;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Ssoma.Clinics
{
    public class ClinicRepository(IDapperHelper dapperHelper) : IClinicRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(Clinic entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.ClinicName,
                    entity.DocumentNumber,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_CLINICS",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@Id");

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }

        public async Task<PagedResult<Clinic>> GetAllAsync(long businessId, string? search, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                Search = search,
                PageNumber = page,
                PageSize = pageSize
            });

            var result = await _dapperHelper.QueryPagedAsync<Clinic>(
                "SP_WS_LIST_CLINICS",
                parameters,
                commandType: CommandType.StoredProcedure);

            return new PagedResult<Clinic>
            {
                Items = result.Items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = result.Total
            };
        }

        public async Task<Clinic?> GetByIdAsync(long clinicId, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                ClinicId = clinicId,
                BusinessId = businessId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<Clinic>(
                "SP_WS_GETBYID_CLINICS",
                parameters);
        }

        public async Task<PagedSelect<ClinicSelectItem?>> GetForSelectAsync(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<ClinicSelectItem>(
                "SP_WS_SELECT_CLINICS",
                parameters);

            return new PagedSelect<ClinicSelectItem?>
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };
        }

        public async Task<BaseResponse> UpdateAsync(Clinic entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.ClinicId,
                    entity.BusinessId,
                    entity.ClinicName,
                    entity.DocumentNumber,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_UPDATE_CLINICS",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long clinicId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    ClinicId = clinicId,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_DELETE_CLINICS",
                    parameters,
                    transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponse
                {
                    Status = cOutput,
                    Message = sOutput
                };
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado.", ex.Message);
            }
        }
    }
}
