using Dapper;
using System.Data;
using System.Data.Common;

namespace Infrastructure.Persistence
{
    public class DapperHelper(ISqlConnectionFactory sqlConnectionFactory) : IDapperHelper
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory = sqlConnectionFactory;

        /// <summary>
        /// Ejecuta de forma asíncrona un procedimiento almacenado o comando SQL y devuelve el número de filas
        /// afectadas.
        /// </summary>
        /// <remarks>Si se proporciona una transacción, el comando se ejecuta en el contexto de dicha
        /// transacción. De lo contrario, se crea una nueva conexión para la ejecución.</remarks>
        /// <param name="sp">El nombre del procedimiento almacenado o la consulta SQL que se va a ejecutar. No puede ser null ni una
        /// cadena vacía.</param>
        /// <param name="param">Un objeto que contiene los parámetros que se pasarán al comando. Puede ser null si no se requieren
        /// parámetros.</param>
        /// <param name="transaction">La transacción de base de datos en la que se ejecutará el comando. Si es null, se crea y utiliza una nueva
        /// conexión.</param>
        /// <param name="commandType">El tipo de comando que se va a ejecutar. El valor predeterminado es CommandType.StoredProcedure.</param>
        /// <returns>Un valor Task que representa la operación asíncrona. El valor de resultado contiene el número de filas
        /// afectadas por la ejecución del comando.</returns>
        public async Task<int> ExecuteAsync(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (transaction != null)
            {
                return await transaction.Connection!.ExecuteAsync(
                    sp, param, transaction, commandType: commandType);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            return await connection.ExecuteAsync(sp, param, commandType: commandType);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado o comando SQL y devuelve el primer valor de la primera fila del conjunto
        /// de resultados de forma asincrónica.
        /// </summary>
        /// <remarks>Si se proporciona una transacción, el comando se ejecuta en la conexión asociada a
        /// dicha transacción. De lo contrario, se crea y utiliza una nueva conexión para la ejecución.</remarks>
        /// <typeparam name="T">El tipo del valor escalar que se va a devolver.</typeparam>
        /// <param name="sp">El nombre del procedimiento almacenado o la consulta SQL que se va a ejecutar.</param>
        /// <param name="param">Un objeto que contiene los parámetros que se pasarán al comando, o null si no se requieren parámetros.</param>
        /// <param name="transaction">Una transacción de base de datos existente en la que ejecutar el comando, o null para ejecutar fuera de una
        /// transacción explícita.</param>
        /// <param name="commandType">El tipo de comando que se va a ejecutar. El valor predeterminado es CommandType.StoredProcedure.</param>
        /// <returns>Un objeto Task que representa la operación asincrónica. El valor de resultado contiene el valor escalar
        /// devuelto convertido a <typeparamref name="T"/>, o null si no se devuelve ningún valor.</returns>
        public async Task<T?> ExecuteScalarAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (transaction != null)
            {
                return await transaction.Connection!.ExecuteScalarAsync<T>(
                    sp, param, transaction, commandType: commandType);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            return await connection.ExecuteScalarAsync<T>(sp, param, commandType: commandType);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado o consulta SQL y devuelve el primer resultado mapeado al tipo
        /// especificado, o el valor predeterminado si no se encuentra ningún resultado.
        /// </summary>
        /// <remarks>Si se proporciona una transacción, la consulta se ejecuta en el contexto de dicha
        /// transacción. De lo contrario, se crea y utiliza una nueva conexión de base de datos.</remarks>
        /// <typeparam name="T">El tipo al que se asignará el resultado de la consulta.</typeparam>
        /// <param name="sp">El nombre del procedimiento almacenado o la consulta SQL que se va a ejecutar.</param>
        /// <param name="param">Un objeto que contiene los parámetros que se pasarán a la consulta. Puede ser null si no se requieren
        /// parámetros.</param>
        /// <param name="transaction">La transacción de base de datos en la que se ejecutará la consulta. Puede ser null para ejecutar fuera de
        /// una transacción.</param>
        /// <param name="commandType">El tipo de comando que se va a ejecutar. El valor predeterminado es CommandType.StoredProcedure.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El valor de la tarea contiene el primer resultado mapeado
        /// al tipo especificado, o el valor predeterminado de T si no se encuentra ningún resultado.</returns>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (transaction != null)
            {
                return await transaction.Connection!.QueryFirstOrDefaultAsync<T>(
                    sp, param, transaction, commandType: commandType);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<T>(sp, param, commandType: commandType);
        }
        /// <summary>
        /// Ejecuta una consulta de forma asíncrona utilizando un procedimiento almacenado o un comando SQL y 
        /// devuelve el resultado mapeado a una colección del tipo especificado.
        /// </summary>
        /// <remarks>Si se proporciona una transacción, la consulta se ejecuta dentro del contexto de dicha transacción.
        /// De lo contrario, se crea una nueva conexión a la base de datos para la operación.</remarks>
        /// <typeparam name="T">El tipo al que se mapean los resultados de la consulta.</typeparam>
        /// <param name="sp">El nombre del procedimiento almacenado o comando SQL a ejecutar.</param>
        /// <param name="param">Un objeto anónimo que contiene los parámetros para el comando. Puede ser nulo si no se requieren parámetros.</param>
        /// <param name="transaction">Una transacción de base de datos opcional en la cual se ejecuta el comando. Si es nula, 
        /// se utiliza una nueva conexión.</param>
        /// <param name="commandType">Especifica cómo se interpreta la cadena del comando. 
        /// El valor predeterminado es CommandType.StoredProcedure.</param>
        /// <returns>Una tarea que representa la operación asíncrona. El resultado de la tarea contiene una colección enumerable de objetos de 
        /// tipo T que representan los resultados de la consulta.</returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (transaction != null)
            {
                return await transaction.Connection!.QueryAsync<T>(
                    sp, param, transaction, commandType: commandType);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            return await connection.QueryAsync<T>(sp, param, commandType: commandType);
        }

        /// <summary>
        /// Ejecuta una consulta paginada utilizando un procedimiento almacenado y devuelve una colección de elementos
        /// junto con el número total de registros coincidentes.
        /// </summary>
        /// <remarks>El método abre y cierra la conexión a la base de datos automáticamente si no se
        /// proporciona una transacción. Si se proporciona una transacción, se utiliza la conexión asociada a dicha
        /// transacción.</remarks>
        /// <typeparam name="T">El tipo de los elementos que se van a recuperar de la base de datos.</typeparam>
        /// <param name="sp">El nombre del procedimiento almacenado que se va a ejecutar.</param>
        /// <param name="param">Un objeto que contiene los parámetros que se pasarán al procedimiento almacenado. Puede ser null si no se
        /// requieren parámetros.</param>
        /// <param name="transaction">Una transacción de base de datos existente en la que se ejecutará la consulta, o null para ejecutar fuera de
        /// una transacción.</param>
        /// <param name="commandType">El tipo de comando que se va a ejecutar. El valor predeterminado es CommandType.StoredProcedure.</param>
        /// <returns>Una tupla que contiene una colección de elementos del tipo especificado y el número total de registros que
        /// cumplen los criterios de la consulta.</returns>
        public async Task<(IEnumerable<T> Items, int Total)> QueryPagedAsync<T>(
            string sp,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (transaction != null)
            {
                return await QueryPagedInternalAsync<T>(
                    transaction.Connection!,
                    sp,
                    param,
                    transaction,
                    commandType);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            return await QueryPagedInternalAsync<T>(
                connection,
                sp,
                param,
                transaction: null,
                commandType);
        }
        /// <summary>
        /// Ejecuta un procedimiento almacenado que devuelve múltiples conjuntos de resultados 
        /// y mapea los resultados de forma asíncrona utilizando la función de mapeo especificada.
        /// </summary>
        /// <remarks>Si se proporciona una transacción, el comando se ejecuta en la conexión asociada dentro de esa transacción.
        /// De lo contrario, se crea una nueva conexión para la operación. 
        /// La función de mapeo es responsable de leer y procesar todos los conjuntos de resultados del GridReader.</remarks>
        /// <typeparam name="TResult">El tipo de resultado devuelto por la función de mapeo después de
        /// procesar los múltiples conjuntos de resultados.</typeparam>
        /// <param name="sp">El nombre del procedimiento almacenado a ejecutar.</param>
        /// <param name="map">Una función que procesa de forma asíncrona los múltiples conjuntos de resultados devueltos por el
        /// procedimiento almacenado y genera un resultado.</param>
        /// <param name="param">Un objeto que contiene los parámetros para el procedimiento almacenado, o nulo si no se requieren parámetros.</param>
        /// <param name="transaction">Una transacción de base de datos opcional para asociar con el comando. 
        /// Si es nula, se crea y utiliza una nueva conexión sin transacción.</param>
        /// <param name="commandType">Especifica cómo se interpreta la cadena del comando. 
        /// El valor predeterminado es CommandType.StoredProcedure.</param>
        /// <returns>Una tarea que representa la operación asíncrona. 
        /// El resultado de la tarea contiene el valor producido por la función de mapeo tras procesar los conjuntos de resultados.</returns>
        public async Task<TResult> QueryMultipleAsync<TResult>(
            string sp,
            Func<SqlMapper.GridReader, Task<TResult>> map,
            object? param = null,
            IDbTransaction? transaction = null,
            CommandType commandType = CommandType.StoredProcedure)
        {
            if (transaction != null)
            {
                using var grid = await transaction.Connection!.QueryMultipleAsync(
                    sp, param, transaction, commandType: commandType);
                return await map(grid);
            }

            using var connection = _sqlConnectionFactory.CreateConnection();
            using var reader = await connection.QueryMultipleAsync(
                sp, param, commandType: commandType);
            return await map(reader);
        }

        private static async Task<(IEnumerable<T> Items, int Total)> QueryPagedInternalAsync<T>(
            IDbConnection connection,
            string sp,
            object? param,
            IDbTransaction? transaction,
            CommandType commandType)
        {
            using var reader = await connection.ExecuteReaderAsync(new CommandDefinition(
                sp,
                param,
                transaction,
                commandType: commandType));

            var dbReader = (DbDataReader)reader;
            var parser = dbReader.GetRowParser<T>();
            var items = new List<T>();
            var total = 0;
            var totalOrdinal = -1;
            var totalResolved = false;

            while (await dbReader.ReadAsync())
            {
                if (!totalResolved)
                {
                    totalOrdinal = TryGetOrdinal(dbReader, "TotalCount");
                    if (totalOrdinal >= 0 && !dbReader.IsDBNull(totalOrdinal))
                    {
                        total = Convert.ToInt32(dbReader.GetValue(totalOrdinal));
                    }

                    totalResolved = true;
                }

                items.Add(parser(dbReader));
            }

            return (items, total);
        }

        private static int TryGetOrdinal(IDataRecord reader, string columnName)
        {
            try
            {
                return reader.GetOrdinal(columnName);
            }
            catch (IndexOutOfRangeException)
            {
                return -1;
            }
        }
    }
}
