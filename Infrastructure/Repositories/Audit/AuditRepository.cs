using Core.Entities.Audit;
using Core.Interfaces.Audit;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using System.Data;

namespace Infrastructure.Repositories.Audit
{
    public class AuditRepository : IAuditRepository
    {
        private readonly IDapperHelper _dapperHelper;

        public AuditRepository(IDapperHelper dapperHelper)
        {
            _dapperHelper = dapperHelper;
        }

        public async Task<long> InsertAuditLogAsync(AuditLog auditLog, IDbTransaction? transaction = null)
        {
            try
            {
                var parameters = DapperParams.From(new
                {
                    auditLog.BusinessId,
                    auditLog.TableName,
                    auditLog.RecordId,
                    auditLog.ActionType,
                    auditLog.ModuleName,
                    auditLog.IpAddress,
                    auditLog.Comment,
                    auditLog.CreateUser
                })
                .WithOutputLong("@Id")
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_AUDIT_LOG",
                    parameters,
                    transaction);

                var code = parameters.Get<int>("@COutput");
                var message = parameters.Get<string>("@SOutput");

                if (code != 1)
                    throw new DatabaseException("No se pudo registrar la cabecera de auditoria.", message);

                return parameters.Get<long>("@Id");
            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al registrar la cabecera de auditoria.", ex.Message);
            }
        }

        public async Task InsertAuditLogDetailAsync(long auditLogId, long userId, long businessId, List<AuditLogDetail> details, IDbTransaction? transaction = null)
        {
            try
            {
                var dataTable = new DataTable();
                dataTable.Columns.Add("FIELD_NAME", typeof(string));
                dataTable.Columns.Add("FIELD_ALIAS", typeof(string));
                dataTable.Columns.Add("OLD_VALUE", typeof(string));
                dataTable.Columns.Add("NEW_VALUE", typeof(string));

                foreach (var item in details)
                {
                    dataTable.Rows.Add(
                        item.FieldName ?? (object)DBNull.Value,
                        item.FieldAlias ?? (object)DBNull.Value,
                        item.OldValue ?? (object)DBNull.Value,
                        item.NewValue ?? (object)DBNull.Value
                    );
                }

                var parameters = DapperParams.From(new
                {
                    AuditLogId = auditLogId,
                    BusinessId = businessId,
                    CreateUser = userId,
                    Details = DbParam.Table(dataTable, "AUDIT_LOG_DETAIL_TYPE")
                })
                .WithOutputInt("@COutput")
                .WithOutputString("@SOutput", 500);

                await _dapperHelper.ExecuteAsync(
                    "SP_WS_INSERT_AUDIT_LOG_DETAIL",
                    parameters,
                    transaction);

                var code = parameters.Get<int>("@COutput");
                var message = parameters.Get<string>("@SOutput");

                if (code != 1)
                    throw new DatabaseException("No se pudo registrar el detalle de auditoria.", message);

            }
            catch (BaseException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al registrar el detalle de auditoria.", ex.Message);
            }
        }
    }
}
