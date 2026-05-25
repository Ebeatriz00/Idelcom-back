using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Infrastructure.Repositories
{
    public class WorkerRepository : IWorkerRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public WorkerRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsAsync(string WorkerDocument, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM WORKER WHERE WORKER_DOCUMENT = @DOC AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                    query.Append(" AND WORKER_ID <> @ID");


                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@DOC", WorkerDocument);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue) cmd.Parameters.AddWithValue("@ID", excludeId.Value);

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia del trabajador.", ex.Message);
            }
        }

        public async Task AddAsync(Worker entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_WORKER", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@AREA_ID", entity.AreaId);
                cmd.Parameters.AddWithValue("@JOB_TITLE_ID", entity.JobTitleId);
                cmd.Parameters.AddWithValue("@PREV_JOB", entity.PrevJob ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DOCUMENT_TYPE_ID", entity.DocumentTypeId);
                cmd.Parameters.AddWithValue("@WORKER_DOCUMENT", entity.WorkerDocument);
                cmd.Parameters.AddWithValue("@WORKER_NAME", entity.WorkerName);
                cmd.Parameters.AddWithValue("@WORKER_LAST_NAME", entity.WorkerLastName);
                cmd.Parameters.AddWithValue("@DEPARTMENT_ID", entity.DepartmentId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PROVINCIA_ID", entity.ProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DISTRICT_ID", entity.DistrictId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ADDRESS", entity.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PHONE", entity.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EMAIL", entity.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BIRTHDATE", entity.BirthDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DATE_ENTRY", entity.DateEntry ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DATE_CES", (object?)entity.DateCes ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@BANK_ID", entity.BankId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CC_BANK", entity.CCBank ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CCI_BANK", entity.CCIBank ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SALARY", entity.Salary ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NUMBER_CHILDREN", entity.NumberChildren ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el trabajador en base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar el trabajador.", ex.Message);
            }
        }

        public async Task<PagedResult<Worker>> GetAllAsync(long businessId, string? search, int page, int pageSize, long? usersBy)
        {
            try
            {
                var list = new List<Worker>();
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_LIST_WORKER", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
                cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PAGE", page);
                cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);
                cmd.Parameters.AddWithValue("@CREATE_USER", (object?)usersBy ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();

                // Primer resultset = items
                while (await reader.ReadAsync())
                {
                    list.Add(new Worker
                    {
                        WorkerId = reader.GetInt64(0),
                        WorkerFullName = reader.GetString(1),
                        DocumentType = reader.GetString(2),
                        WorkerDocument = reader.GetString(3),
                        DateEntry = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                        District = reader.IsDBNull(5) ? null : reader.GetString(5),
                        Address = reader.IsDBNull(6) ? null : reader.GetString(6),
                        JobTitle = reader.GetString(7),
                        Status = reader.GetString(8),
                        WorkerCount = reader.GetInt32(reader.GetOrdinal("WorkerCount"))
                    });
                }

                // Segundo resultset = total
                int total = 0;
                if (await reader.NextResultAsync() && await reader.ReadAsync())
                {
                    total = reader.GetInt32(0);
                }

                return new PagedResult<Worker>
                {
                    Items = list,
                    Page = page,
                    PageSize = pageSize,
                    Total = total
                };
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener lista de trabajadores paginada.", ex.Message);
            }
        }

        public async Task<PagedSelect<OptionItem>> GetWorkerForSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_WORKER_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                items.Add(new OptionItem
                {
                    Value = dr.GetInt64(0),
                    Label = dr.GetString(1)
                });
            }

            var hasMore = items.Count > pageSize;
            if (hasMore) items.RemoveAt(items.Count - 1);

            return new PagedSelect<OptionItem>
            {
                Items = items,
                HasMore = hasMore,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedSelect<OptionItem>> GetWorkerSalesSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_WORKER_SALES_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                items.Add(new OptionItem
                {
                    Value = dr.GetInt64(0),
                    Label = dr.GetString(1)
                });
            }

            var hasMore = items.Count > pageSize;
            if (hasMore) items.RemoveAt(items.Count - 1);

            return new PagedSelect<OptionItem>
            {
                Items = items,
                HasMore = hasMore,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedSelect<OptionItem>> GetWorkerProyectsSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_WORKER_PROYECT_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                items.Add(new OptionItem
                {
                    Value = dr.GetInt64(0),
                    Label = dr.GetString(1)
                });
            }

            var hasMore = items.Count > pageSize;
            if (hasMore) items.RemoveAt(items.Count - 1);

            return new PagedSelect<OptionItem>
            {
                Items = items,
                HasMore = hasMore,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PagedSelect<OptionItem>> GetWorkerOperationsSelectAsync(long businessId, string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_WORKER_OPERATIONS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                items.Add(new OptionItem
                {
                    Value = dr.GetInt64(0),
                    Label = dr.GetString(1)
                });
            }

            var hasMore = items.Count > pageSize;
            if (hasMore) items.RemoveAt(items.Count - 1);

            return new PagedSelect<OptionItem>
            {
                Items = items,
                HasMore = hasMore,
                Page = page,
                PageSize = pageSize
            };
        }


        public async Task<Worker> GetByIdAsync(long workerId)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_WORKER_BY_ID", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@WORKER_ID", workerId);

                using var reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Worker
                    {
                        WorkerId = reader.GetInt64(0),
                        BusinessId = reader.GetInt64(1),
                        AreaId = reader.GetInt64(2),
                        JobTitleId = reader.GetInt64(3),
                        PrevJob = reader.IsDBNull(4) ? null : reader.GetString(4),
                        DocumentTypeId = reader.GetInt64(5),
                        WorkerDocument = reader.GetString(6),
                        WorkerName = reader.GetString(7),
                        WorkerLastName = reader.GetString(8),
                        DepartmentId = reader.IsDBNull(9) ? null : reader.GetInt64(9),
                        ProvinceId = reader.IsDBNull(10) ? null : reader.GetInt64(10),
                        DistrictId = reader.IsDBNull(11) ? null : reader.GetInt64(11),
                        Address = reader.IsDBNull(12) ? null : reader.GetString(12),
                        Phone = reader.IsDBNull(13) ? null : reader.GetString(13),
                        Email = reader.IsDBNull(14) ? null : reader.GetString(14),
                        BirthDate = reader.IsDBNull(15) ? (DateTime?)null : reader.GetDateTime(15),
                        DateEntry = reader.IsDBNull(16) ? (DateTime?)null : reader.GetDateTime(16),
                        DateCes = reader.IsDBNull(17) ? (DateTime?)null : reader.GetDateTime(17),
                        BankId = reader.IsDBNull(18) ? null : reader.GetInt64(18),
                        CCBank = reader.IsDBNull(19) ? null : reader.GetString(19),
                        CCIBank = reader.IsDBNull(20) ? null : reader.GetString(20),
                        Salary = reader.IsDBNull(21) ? null : reader.GetDecimal(21),
                        NumberChildren = reader.IsDBNull(22) ? null : reader.GetString(22)
                    };
                }
                return null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al obtener trabajador por ID.", ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(Worker entity)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();

                using var cmd = new SqlCommand("SP_WS_UPDATE_WORKER", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@WORKER_ID", entity.WorkerId);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@AREA_ID", entity.AreaId);
                cmd.Parameters.AddWithValue("@JOB_TITLE_ID", entity.JobTitleId);
                cmd.Parameters.AddWithValue("@PREV_JOB", entity.PrevJob ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DOCUMENT_TYPE_ID", entity.DocumentTypeId);
                cmd.Parameters.AddWithValue("@WORKER_DOCUMENT", entity.WorkerDocument);
                cmd.Parameters.AddWithValue("@WORKER_NAME", entity.WorkerName);
                cmd.Parameters.AddWithValue("@WORKER_LAST_NAME", entity.WorkerLastName);
                cmd.Parameters.AddWithValue("@DEPARTMENT_ID", entity.DepartmentId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PROVINCIA_ID", entity.ProvinceId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DISTRICT_ID", entity.DistrictId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ADDRESS", entity.Address ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@PHONE", entity.Phone ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@EMAIL", entity.Email ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BIRTHDATE", entity.BirthDate ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DATE_ENTRY", entity.DateEntry ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@DATE_CES", entity.DateCes ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@BANK_ID", entity.BankId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CC_BANK", entity.CCBank ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@CCI_BANK", entity.CCIBank ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SALARY", entity.Salary ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue(@"NUMBER_CHILDREN", entity.NumberChildren ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UPDATE_USER", entity.UsersBy);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar el trabajador.", ex.Message);
            }
        }

        public async Task<bool> PatchStatusAsync(long workerId, string status, long UsersBy, long businessId)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_UPDATE_WORKER_STATUS", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@WORKER_ID", workerId);
                cmd.Parameters.AddWithValue("@STATUS", status);
                cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
                cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);

                await connection.OpenAsync();
                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al actualizar estado del trabajador.", ex.Message);
            }
        }


    }
}
