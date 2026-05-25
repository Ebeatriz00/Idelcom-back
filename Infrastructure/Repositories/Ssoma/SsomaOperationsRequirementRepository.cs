using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Interfaces.Ssoma;
using Core.Projections.SsomaOperationsRequirement;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Ssoma
{
    public class SsomaOperationsRequirementRepository(IDapperHelper dapperHelper) : ISsomaOperationsRequirementRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(SsomaOperationsRequirement entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.OperationsId,
                    entity.RequirementId,
                    entity.IsMandatory,
                    entity.ValidDays,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_SSOMA_OPERATIONS_REQUIREMENT", parameters, transaction);

                var cOutput = parameters.Get<int>("@COutput");
                var sOutput = parameters.Get<string>("@SOutput");
                var id = parameters.Get<long>("@Id");

                if (cOutput != 1)
                    throw new BusinessException(sOutput);

                return new BaseResponseId
                {
                    Status = cOutput,
                    Message = sOutput,
                    Id = id
                };
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado en la creación del registro.", ex.Message);
            }
        }


        public async Task<PagedResult<SsomaOperationsRequirementItem>> GetAllAsync(long businessId, long operationsId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaOperationsRequirementItem>(
                "SP_WS_LIST_SSOMA_OPERATIONS_REQUIREMENT",
                parameters);

            return new PagedResult<SsomaOperationsRequirementItem>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<PagedResult<SsomaOperationsRequirementByWorkerItem>> GetListByWorkerAsync(long businessId, long operationsId, long workerId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                WorkerId = workerId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaOperationsRequirementByWorkerItem>(
                "SP_WS_LIST_SSOMA_OPERATIONS_REQUIREMENT_BY_WORKER",
                parameters);

            return new PagedResult<SsomaOperationsRequirementByWorkerItem>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<ValidateSsomaOperationsRequirementByWorkerItem?> ValidateByWorkerAsync(long businessId, long operationsId, long workerId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                WorkerId = workerId
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<ValidateSsomaOperationsRequirementByWorkerItem>(
                "SP_WS_VALIDATE_SSOMA_OPERATIONS_REQUIREMENT_BY_WORKER",
                parameters);
        }

        public async Task<IEnumerable<SsomaOperationsRequirementMissingByWorkerItem>> GetMissingByWorkerAsync(long businessId, long operationsId, long workerId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                WorkerId = workerId
            });

            return await _dapperHelper.QueryAsync<SsomaOperationsRequirementMissingByWorkerItem>(
                "SP_WS_LIST_SSOMA_OPERATIONS_REQUIREMENT_MISSING_BY_WORKER",
                parameters);
        }

        public async Task<PagedSelect<OptionItem>> GetForSelectOperationsAsync(long businessId, string? search, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<OptionItem>(
                "SP_WS_SELECT_OPERATIONS_FOR_HOMOLOGATION",
                parameters);

            return new PagedSelect<OptionItem>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = (page * pageSize) < total
            };
        }

        public async Task<BaseResponse> DeleteAsync(long id, long businessId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    SsomaOperationsRequirementId = id,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DELETE_SSOMA_OPERATIONS_REQUIREMENT", parameters, transaction);

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
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error al intentar eliminar el requerimiento.", ex.Message);
            }
        }

        public async Task<SsomaOperationsRequirement?> GetByIdAsync(long id, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                SsomaOperationsRequirementId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaOperationsRequirement>(
                "SP_WS_GETBYID_SSOMA_OPERATIONS_REQUIREMENT",
                parameters);
        }

        public async Task<SsomaOperationsRequirement?> GetByIdAsync(long id, long businessId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                SsomaOperationsRequirementId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaOperationsRequirement>(
                "SP_WS_GETBYID_SSOMA_OPERATIONS_REQUIREMENT",
                parameters,
                transaction);
        }

        public async Task<BaseResponse> UpdateAsync(SsomaOperationsRequirement entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.SsomaOperationsRequirementId,
                    entity.BusinessId,
                    entity.OperationsId,
                    entity.RequirementId,
                    entity.IsMandatory,
                    entity.ValidDays,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_SSOMA_OPERATIONS_REQUIREMENT", parameters, transaction);

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
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al actualizar el registro.", ex.Message);
            }
        }
    }
}
