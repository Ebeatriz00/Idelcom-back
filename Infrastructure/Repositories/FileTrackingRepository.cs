using Core.Entities;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class FileTrackingRepository : IFileTrackingRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public FileTrackingRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ExistsOpporAsync(string fileUrl, string opporToken, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM FILE_TRACKING WHERE FILE_URL = @FURL AND OPPOR_ID = @OID AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                {
                    query.Append(" AND FILE_TRACKING_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@OID", opporToken);
                cmd.Parameters.AddWithValue("@FURL", fileUrl);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                }

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de archivos.", ex.Message);
            }
        }


        public async Task<bool> ExistsProjectAsync(string fileUrl, string projectToken, long businessId, long? excludeId = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var query = new StringBuilder("SELECT COUNT(*) FROM FILE_TRACKING WHERE FILE_URL = @FURL AND PROJECT_ID = @PID AND BUSINESS_ID = @BID");

                if (excludeId.HasValue)
                {
                    query.Append(" AND FILE_TRACKING_ID <> @ID");
                }

                using var cmd = new SqlCommand(query.ToString(), connection);
                cmd.Parameters.AddWithValue("@PID", projectToken);
                cmd.Parameters.AddWithValue("@FURL", fileUrl);
                cmd.Parameters.AddWithValue("@BID", businessId);
                if (excludeId.HasValue)
                {
                    cmd.Parameters.AddWithValue("@ID", excludeId.Value);
                }

                await connection.OpenAsync();
                var count = (int)await cmd.ExecuteScalarAsync();
                return count > 0;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al validar existencia de archivos.", ex.Message);
            }
        }



        public async Task AddFileOpporAsync(FileTracking entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_FILE_TRACKING_OPPOR", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", entity.OpporToken);
                cmd.Parameters.AddWithValue("FILE_TYPE", entity.ArchiveType);
                cmd.Parameters.AddWithValue("@FILE_TITLE", entity.FileTitle);
                cmd.Parameters.AddWithValue("@RELATIVE_PATH", (object?)(string.IsNullOrWhiteSpace(entity.RelativePath) ? null : entity.RelativePath.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILE_URL", entity.FileUrl);
                cmd.Parameters.AddWithValue("@COMMENT", (object?)(string.IsNullOrWhiteSpace(entity.Comment) ? null : entity.Comment.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el archivo de una oportunidad en la base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar un archivo de una oportunidad.", ex.Message);
            }
        }


        public async Task AddFileProjectAsync(FileTracking entity)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var cmd = new SqlCommand("SP_WS_REGISTER_FILE_TRACKING_PROJECT", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };

                cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
                cmd.Parameters.AddWithValue("@PROJECT_ID", entity.ProjectToken);
                cmd.Parameters.AddWithValue("@FILE_TITLE", entity.FileTitle);
                cmd.Parameters.AddWithValue("@RELATIVE_PATH", (object?)(string.IsNullOrWhiteSpace(entity.RelativePath) ? null : entity.RelativePath.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@FILE_URL", entity.FileUrl);
                cmd.Parameters.AddWithValue("@COMMENT", (object?)(string.IsNullOrWhiteSpace(entity.Comment) ? null : entity.Comment.Trim()) ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

                await connection.OpenAsync();
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al registrar el archivo de un proyecto en la base de datos.", ex.Message);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Error inesperado al guardar un archivo de un proyecto.", ex.Message);
            }
        }



        public async Task<bool> DeleteFileOpporAsync(string linkToken, string OpporToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_DELETE_FILE_TRACKING_OPPOR", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@FILE_TRACKING_ID", linkToken);
                cmd.Parameters.AddWithValue("@OPPOR_ID", OpporToken);

                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar el archivo de una oportunidad.", ex.Message);
            }
        }



        public async Task<bool> DeleteFileProjectAsync(string linkToken, string projectToken)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_DELETE_FILE_TRACKING_PROJECT", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@FILE_TRACKING_ID", linkToken);
                cmd.Parameters.AddWithValue("@PROJECT_ID", projectToken);


                using var reader = await cmd.ExecuteReaderAsync();
                return reader.HasRows && await reader.ReadAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error al eliminar el archivo de un proyecto.", ex.Message);
            }
        }
    }
}
