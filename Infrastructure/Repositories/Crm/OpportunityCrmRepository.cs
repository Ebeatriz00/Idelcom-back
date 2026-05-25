using Core.Interfaces.Crm;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.Crm
{
    public class OpportunityCrmRepository(IDapperHelper dapperHelper) 
    {
        private readonly IDapperHelper _dapperHelper = dapperHelper;

        public async Task<bool> ExistsAsync(string oppDesc, long businessId, long? excludeId = null)
        {
            try
            {
                var parameters = DapperParams.From()
                    .WithInput("@DESC", oppDesc)
                    .WithInput("@BID", businessId)
                    .WithInput("@ID", excludeId);

                const string query = """
                    SELECT COUNT(*)
                    FROM dbo.OPPORTUNITY
                    WHERE OPPOR_DESC LIKE '%' + @DESCRIPTION + '%'
                      AND BUSINESS_ID = @BID
                      AND (@ID IS NULL OR OPPOR_ID <> @ID)
                    """;

                var count = await _dapperHelper.ExecuteScalarAsync<int>(
                    query,
                    parameters,
                    commandType: CommandType.Text);

                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de la oportunidad.", ex.Message);
            }
        }

    }
}
