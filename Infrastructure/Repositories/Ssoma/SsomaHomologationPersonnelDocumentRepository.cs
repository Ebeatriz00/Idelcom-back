using Core.Entities.paginations;
using Core.Entities.Ssoma;
using Core.Interfaces.Ssoma;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using SharedKernel;
using System.Data;

namespace Infrastructure.Repositories.Ssoma
{
    public class SsomaHomologationPersonnelDocumentRepository(IDapperHelper dapperHelper) : ISsomaHomologationPersonnelDocumentRepository
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<BaseResponseId> CreateAsync(SsomaHomologationPersonnelDocument entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.BusinessId,
                    entity.HomologationPersonnelId,
                    entity.RequirementId,
                    entity.FileName,
                    entity.FileUrl,
                    entity.FilePath,
                    entity.IssueDate,
                    entity.ExpirationDate,
                    entity.ValidationStatusId,
                    entity.ReviewDate,
                    entity.Observation,
                    entity.ReplacedDocumentId,
                    entity.DocumentVersion,
                    entity.ReplacementReason,
                    entity.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_INSERT_SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT", parameters, transaction);

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
                throw new DatabaseException("Error inesperado en la creación del documento.", ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteAsync(long id, long businessId, long userId, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    SsomaHomologationPersonnelDocumentId = id,
                    UpdateUser = userId
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_DELETE_SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT", parameters, transaction);

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
                throw new DatabaseException("Error inesperado al eliminar el documento.", ex.Message);
            }
        }

        public async Task<PagedResult<SsomaHomologationPersonnelDocument>> GetAllAsync(
            long businessId,
            long? homologationPersonnelId,
            int? requirementId,
            int page,
            int pageSize,
            string? search)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                HomologationPersonnelId = homologationPersonnelId,
                RequirementId = requirementId,
                PageNumber = page,
                PageSize = pageSize,
                Search = search
            });

            var (items, total) = await _dapperHelper.QueryPagedAsync<SsomaHomologationPersonnelDocument>(
                "SP_WS_GETALL_SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT",
                parameters);

            return new PagedResult<SsomaHomologationPersonnelDocument>
            {
                Items = items.ToList(),
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }

        public async Task<SsomaHomologationPersonnelDocument?> GetByIdAsync(long id, long businessId)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                SsomaHomologationPersonnelDocumentId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaHomologationPersonnelDocument>(
                "SP_WS_GETBYID_SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT",
                parameters);
        }

        public async Task<SsomaHomologationPersonnelDocument?> GetByIdAsync(long id, long businessId, IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                SsomaHomologationPersonnelDocumentId = id
            });

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaHomologationPersonnelDocument>(
                "SP_WS_GETBYID_SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT",
                parameters,
                transaction);
        }

        public async Task<SsomaHomologationPersonnelDocument?> GetActiveByHomologationAndRequirementAsync(
            long businessId,
            long homologationPersonnelId,
            int requirementId,
            IDbTransaction transaction)
        {
            var parameters = DapperParams.From(new
            {
                BusinessId = businessId,
                HomologationPersonnelId = homologationPersonnelId,
                RequirementId = requirementId
            });

            var query = @"
                SELECT TOP (1)
                    SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT_ID AS SsomaHomologationPersonnelDocumentId,
                    BUSINESS_ID AS BusinessId,
                    HOMOLOGATION_PERSONNEL_ID AS HomologationPersonnelId,
                    REQUIREMENT_ID AS RequirementId,
                    FILE_NAME AS FileName,
                    FILE_URL AS FileUrl,
                    FILE_PATH AS FilePath,
                    ISSUE_DATE AS IssueDate,
                    EXPIRATION_DATE AS ExpirationDate,
                    VALIDATION_STATUS_ID AS ValidationStatusId,
                    REVIEW_DATE AS ReviewDate,
                    OBSERVATION AS Observation,
                    REPLACED_DOCUMENT_ID AS ReplacedDocumentId,
                    DOCUMENT_VERSION AS DocumentVersion,
                    REPLACEMENT_REASON AS ReplacementReason,
                    CREATE_DATE AS CreateDate,
                    CREATE_USER AS CreateUser,
                    UPDATE_DATE AS UpdateDate,
                    UPDATE_USER AS UpdateUser,
                    STATUS AS Status
                FROM dbo.SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT WITH (UPDLOCK, HOLDLOCK)
                WHERE BUSINESS_ID = @BusinessId
                  AND HOMOLOGATION_PERSONNEL_ID = @HomologationPersonnelId
                  AND REQUIREMENT_ID = @RequirementId
                  AND STATUS = '1'
                ORDER BY SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT_ID DESC;";

            return await _dapperHelper.QueryFirstOrDefaultAsync<SsomaHomologationPersonnelDocument>(
                query,
                parameters,
                transaction,
                CommandType.Text);
        }

        public async Task<BaseResponse> UpdateAsync(SsomaHomologationPersonnelDocument entity, IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    entity.SsomaHomologationPersonnelDocumentId,
                    entity.BusinessId,
                    entity.HomologationPersonnelId,
                    entity.RequirementId,
                    entity.FileName,
                    entity.FileUrl,
                    entity.FilePath,
                    entity.IssueDate,
                    entity.ExpirationDate,
                    entity.ValidationStatusId,
                    entity.ReviewDate,
                    entity.Observation,
                    entity.ReplacedDocumentId,
                    entity.DocumentVersion,
                    entity.ReplacementReason,
                    entity.UpdateUser
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync("SP_WS_UPDATE_SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT", parameters, transaction);

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
                throw new DatabaseException("Error inesperado al actualizar el documento.", ex.Message);
            }
        }

        public async Task<BaseResponse> MarkAsReplacedAsync(
            long id,
            long businessId,
            long replacedDocumentId,
            string? replacementReason,
            long userId,
            IDbTransaction transaction)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    BusinessId = businessId,
                    SsomaHomologationPersonnelDocumentId = id,
                    ReplacedDocumentId = replacedDocumentId,
                    ReplacementReason = replacementReason,
                    UpdateUser = userId
                });

                var query = @"
                    UPDATE dbo.SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT
                    SET REPLACED_DOCUMENT_ID = @ReplacedDocumentId,
                        REPLACEMENT_REASON = @ReplacementReason,
                        UPDATE_USER = @UpdateUser,
                        UPDATE_DATE = GETDATE(),
                        STATUS = '0'
                    WHERE BUSINESS_ID = @BusinessId
                      AND SSOMA_HOMOLOGATION_PERSONNEL_DOCUMENT_ID = @SsomaHomologationPersonnelDocumentId
                      AND STATUS = '1';";

                var affectedRows = await _dapperHelper.ExecuteAsync(query, parameters, transaction, CommandType.Text);
                if (affectedRows != 1)
                    throw new BusinessException("No se pudo marcar el documento anterior como reemplazado.");

                return new BaseResponse
                {
                    Status = 1,
                    Message = "Documento anterior marcado como reemplazado correctamente."
                };
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al marcar el documento como reemplazado.", ex.Message);
            }
        }
    }
}
