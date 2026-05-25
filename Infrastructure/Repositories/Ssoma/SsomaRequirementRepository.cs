using Core.Entities.Operations;
using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Interfaces.Ssoma;
using Core.Projections.Operations;
using Core.Projections.SsomRequirement;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Ssoma
{
    public class SsomaRequirementRepository(IDapperHelper dapperHelper) : ISsomaRequirementRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;
      public async Task<BaseResponseId> CreateAsync(SsomaRequirement entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.Name,
                    entity.Description,
                    entity.Duration,
                    entity.ScopeId,
                    entity.RequiresFile,
                    entity.RequiresExpiration,
                    entity.MaxFileSize,
                    entity.HasExpiration,
                    entity.AllowedExtensions,
                    entity.AllowInternalReuse,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_SSOMA_REQUIREMENT", parameters, transaction);

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

        public async Task<BaseResponse> DeleteAsync(long id, long businessId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    RequirementId = id,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DELETE_SSOMA_REQUIREMENT", parameters, transaction);

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
                throw new DatabaseException("Error inesperado al eliminar el registro.", ex.Message);
            }
        }

        public async Task<SsomaRequirement?> GetByIdAsync(long id, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                RequirementId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaRequirement>("SP_WS_GETBYID_SSOMA_REQUIREMENT", parameters);
        }

        public async Task<SsomaRequirement?> GetByIdAsync(long id, long businessId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                RequirementId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaRequirement>(
                "SP_WS_GETBYID_SSOMA_REQUIREMENT",
                parameters,
                transaction);
        }

        public async Task<BaseResponse> UpdateAsync(SsomaRequirement entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.RequirementId,
                    entity.BusinessId,
                    entity.Name,
                    entity.Description,
                    entity.Duration,
                    entity.ScopeId,
                    entity.RequiresFile,
                    entity.RequiresExpiration,
                    entity.MaxFileSize,
                    entity.AllowedExtensions,
                    entity.AllowInternalReuse,
                    entity.HasExpiration,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_SSOMA_REQUIREMENT", parameters, transaction);

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

        public async Task<PagedResult<SsomaRequirement>> GetAllAsync(long businessId, int scopeId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                ScopeId = scopeId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaRequirement>(
                "SP_WS_GET_ALL_SSOMA_REQUIREMENT", 
                parameters);

            return new PagedResult<SsomaRequirement>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<PagedResult<SsomaRequirementItem>> GetAllItemAsync( long businessId,  int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaRequirementItem>(
                "SP_WS_LIST_SSOMA_REQUIREMENT",
                parameters);

            return new PagedResult<SsomaRequirementItem>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }


        public async Task<PagedSelect<OptionItem>> GetForSelectAsync(long businessId, int scopedId, string? search, int page, int pageSize)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                ScopeId = scopedId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var result = await _dapperHelper.QueryAsync<OptionItem>("SP_WS_SELECT_SSOMA_REQUIREMENT_BY_SCOPE", parameters);

            return new()
            {
                Items = result.ToList(),
                Page = page,
                PageSize = pageSize,
                HasMore = false
            };

        }

    }

}
