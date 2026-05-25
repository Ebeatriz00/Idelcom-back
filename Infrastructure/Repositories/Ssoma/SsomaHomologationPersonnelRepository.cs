using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Interfaces.Ssoma;
using Core.Projections.Ssoma;
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
     public class SsomaHomologationPersonnelRepository(IDapperHelper dapperHelper) : ISsomaHomologationPersonnelRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(SsomaHomologationPersonnel entity,  IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.HomologationScopeId,
                    entity.OperationsId,
                    entity.WorkerId,
                    entity.WorkerStatusId,
                    entity.MedicalAptitudeId,
                    entity.ValidFrom,
                    entity.ValidTo,
                    entity.SsomaApproved,
                    entity.Notes,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_SSOMA_HOMOLOGATION_PERSONNEL", parameters, transaction);

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
                    HomologationPersonnelId = id,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DELETE_SSOMA_HOMOLOGATION_PERSONNEL", parameters, transaction);

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

        public async Task<PagedResult<SsomaHomologationPersonnel>> GetAllAsync(long businessId, long? operationsId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                OperationsId = operationsId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaHomologationPersonnel>(
                "SP_WS_GETALL_SSOMA_HOMOLOGATION_PERSONNEL",
                parameters);

            return new PagedResult<SsomaHomologationPersonnel>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<SsomaHomologationPersonnel?> GetByIdAsync(long id, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                HomologationPersonnelId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaHomologationPersonnel>("SP_WS_GETBYID_SSOMA_HOMOLOGATION_PERSONNEL", parameters);
        }

        public async Task<SsomaHomologationPersonnel?> GetByIdAsync(long id, long businessId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                HomologationPersonnelId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaHomologationPersonnel>(
                "SP_WS_GETBYID_SSOMA_HOMOLOGATION_PERSONNEL",
                parameters,
                transaction);
        }

        public async Task<SsomaHomologationPersonnel?> GetActiveByBusinessWorkerScopeAsync(
            long businessId,
            long workerId,
            long homologationScopeId,
            long? operationsId,
            IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                WorkerId = workerId,
                HomologationScopeId = homologationScopeId,
                OperationsId = operationsId
            });

            var query = @"
                SELECT TOP (1)
                    HOMOLOGATION_PERSONNEL_ID AS HomologationPersonnelId,
                    BUSINESS_ID AS BusinessId,
                    HOMOLOGATION_SCOPE_ID AS HomologationScopeId,
                    OPERATIONS_ID AS OperationsId,
                    WORKER_ID AS WorkerId,
                    WORKER_STATUS_ID AS WorkerStatusId,
                    MEDICAL_APTITUDE_ID AS MedicalAptitudeId,
                    VALID_FROM AS ValidFrom,
                    VALID_TO AS ValidTo,
                    SSOMA_APPROVED AS SsomaApproved,
                    NOTES AS Notes,
                    CREATE_DATE AS CreateDate,
                    CREATE_USER AS CreateUser,
                    UPDATE_DATE AS UpdateDate,
                    UPDATE_USER AS UpdateUser,
                    STATUS AS Status
                FROM dbo.SSOMA_HOMOLOGATION_PERSONNEL WITH (UPDLOCK, HOLDLOCK)
                WHERE BUSINESS_ID = @BusinessId
                  AND WORKER_ID = @WorkerId
                  AND HOMOLOGATION_SCOPE_ID = @HomologationScopeId
                  AND STATUS = '1'
                  AND (
                        (@HomologationScopeId = 1 AND OPERATIONS_ID IS NULL)
                        OR (@HomologationScopeId = 2 AND OPERATIONS_ID = @OperationsId)
                      )
                ORDER BY HOMOLOGATION_PERSONNEL_ID DESC;";

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaHomologationPersonnel>(
                query,
                parameters,
                transaction,
                CommandType.Text);
        }

        public async Task<BaseResponse> UpdateAsync(SsomaHomologationPersonnel entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.HomologationPersonnelId,
                    entity.BusinessId,
                    entity.HomologationScopeId,
                    entity.OperationsId,
                    entity.WorkerId,
                    entity.WorkerStatusId,
                    entity.MedicalAptitudeId,
                    entity.ValidFrom,
                    entity.ValidTo,
                    entity.SsomaApproved,
                    entity.Notes,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_SSOMA_HOMOLOGATION_PERSONNEL", parameters, transaction);

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

        public async Task<PagedResult<SsomaPersonnelOperationsListItem>> GetListAllPersonnelOperations(long businessId, int page, int pageSize, string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });
            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaPersonnelOperationsListItem>(
                "SP_WS_LIST_SSOMA_PERSONNEL_OPERATIONS",
                parameters,
                commandType: CommandType.StoredProcedure);

            return new PagedResult<SsomaPersonnelOperationsListItem>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<SsomaPersonnelOperationsItem?> GetDetailPersonnelOperations(long personnelOperationsId, long businessId)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    PersonnelOperationsId = personnelOperationsId,
                    BusinessId = businessId
                });

                return await _dapperHelper.QueryMultipleAsync(
                    "SP_WS_GETDETAIL_SSOMA_PERSONNEL_OPERATIONS",
                    async grid =>
                    {
                        var header = await grid.ReadFirstOrDefaultAsync<SsomaPersonnelOperationsHeaderRow>();

                        if (header == null)
                            return null;

                        var generalItems = (await grid.ReadAsync<SsomaPersonnelHomologationGeneralItem>())
                            .Where(x =>
                                !string.IsNullOrWhiteSpace(x.Requeriment) ||
                                !long.IsEvenInteger(x.RequirementId) ||
                                !long.IsEvenInteger(x.HomologationPersonnelId) ||
                                !string.IsNullOrWhiteSpace(x.FileName) ||
                                !string.IsNullOrWhiteSpace(x.ValidationStatus) ||
                                !string.IsNullOrWhiteSpace(x.FileUrl))
                            .ToList();

                        var operationsItems = (await grid.ReadAsync<SsomaPersonnelHomologationOperationsItem>())
                            .Where(x =>
                                !long.IsEvenInteger(x.OperationsId) ||
                                !string.IsNullOrWhiteSpace(x.OperationsName) ||
                                !string.IsNullOrWhiteSpace(x.Requeriment) ||
                                !long.IsEvenInteger(x.RequirementId) ||
                                !long.IsEvenInteger(x.HomologationPersonnelId) ||
                                !string.IsNullOrWhiteSpace(x.FileName) ||
                                !string.IsNullOrWhiteSpace(x.ValidationStatus) ||
                                !string.IsNullOrWhiteSpace(x.FileUrl))
                            .ToList();
                        var summaryItems = (await grid.ReadAsync<SsomaPersonnelHomologationsSummaryItem>())
                            .Where(x => 
                            !int.IsEvenInteger(x.ActiveProject) ||
                            !int.IsEvenInteger(x.CurrentDocuments) ||
                            !int.IsEvenInteger(x.TotalDocuments) ||
                            !string.IsNullOrWhiteSpace(x.SummaryCurrentDocuments) ||
                            !int.IsEvenInteger(x.Pendings) ||
                            !int.IsEvenInteger(x.Observations) ||
                            !int.IsEvenInteger(x.Expired) ||
                            !int.IsEvenInteger(x.ToExpired) ||
                            !int.IsEvenInteger(x.GeneralShortages) ||
                            !int.IsEvenInteger(x.OperationsShortages) ||
                            !int.IsEvenInteger(x.Shortages) ||
                            !int.IsEvenInteger(x.GeneralPercentage) 
                            ).ToList();
                        return new SsomaPersonnelOperationsItem
                        {
                            PersonnelOperationsId = header.PersonnelOperationsId,
                            PersonnelFullName = header.PersonnelFullName ?? string.Empty,
                            PersonnelDocument = header.PersonnelDocument ?? string.Empty,
                            PersonnelPosittion = header.PersonnelPosittion ?? string.Empty,
                            Status = header.Status,
                            PersonnelHomologationGeneralItems = generalItems,
                            PersonnelHomologationOperationsItem = operationsItems,
                            PersonnelHomologationSummaryItem = summaryItems,
                        };
                    },
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al obtener el detalle del personal de operaciones.", ex.Message);
            }
        }

    }

}
