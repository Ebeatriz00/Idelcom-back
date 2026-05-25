using Dapper;
using System.Data;

namespace Infrastructure.Persistence
{
    public interface IDapperHelper
    {
        Task<int> ExecuteAsync(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure);

        Task<T?> ExecuteScalarAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure);

        Task<T?> QueryFirstOrDefaultAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure);

        Task<IEnumerable<T>> QueryAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure);

        Task<(IEnumerable<T> Items, int Total)> QueryPagedAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure);

        Task<TResult> QueryMultipleAsync<TResult>(
            string sp,
            Func<SqlMapper.GridReader, Task<TResult>> map,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure);
    }
}
