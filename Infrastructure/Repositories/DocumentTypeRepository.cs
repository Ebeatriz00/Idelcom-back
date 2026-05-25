using Azure;
using Core.Entities;
using Core.Entities.paginations;
using Core.Interfaces;
using Infrastructure.Exceptions;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Infrastructure.Persistence.Repositories;

public class DocumentTypeRepository : IDocumentTypeRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DocumentTypeRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(DocumentType entity)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            using var cmd = new SqlCommand("SP_WS_REGISTER_DOCUMENT_TYPE", connection)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", entity.BusinessId);
            cmd.Parameters.AddWithValue("@SUNAT_CODE", entity.CodeSunat);
            cmd.Parameters.AddWithValue("@DESCRIPTION", entity.Description);
            cmd.Parameters.AddWithValue("@ABRV", entity.Abrv);
            cmd.Parameters.AddWithValue("@CREATE_USER", entity.UsersBy);

            await connection.OpenAsync();
            await cmd.ExecuteNonQueryAsync();
            var newId = await cmd.ExecuteScalarAsync();
        }
        catch (SqlException ex)
        {
            throw new DatabaseException("Error al registrar tipo de documento en base de datos.", ex.Message);
        }
        catch (Exception ex)
        {
            throw new DatabaseException("Error inesperado al guardar tipo de documento.", ex.Message);
        }
    }

    public async Task<PagedResult<DocumentType>> GetAllAsync(int businessId, string? search, int page, int pageSize, long? usersBy)
    {
        try
        {
            var list = new List<DocumentType>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_DOCUMENT_TYPE_LIST", cn)
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
            while (await reader.ReadAsync())
            {
                list.Add(new DocumentType
                {
                    BusinessId = reader.GetInt64(0),
                    DocumentTypeId = reader.GetInt64(1),
                    CodeSunat = reader.GetString(2),
                    Description = reader.GetString(3),
                    Status = reader.GetString(4),
                    DocumentTypeCount = reader.GetInt32(5),
                    Abrv = reader.IsDBNull(6) ? null : reader.GetString(6),
                });
            }
            int total = 0;
            if (await reader.NextResultAsync() && await reader.ReadAsync())
            {
                total = reader.GetInt32(0);
            }

            return new PagedResult<DocumentType>
            {
                Items = list,
                Page = page,
                PageSize = pageSize,
                Total = total
            };
        }
        catch (SqlException ex)
        {
            throw new DatabaseException("Error al obtener lista de tipos de documentos.", ex.Message);
        }
    }


    public async Task<PagedSelect<OptionItem>> GetAreaForSelectAsync(long businessId, string? search, int page, int pageSize)
    {
        var items = new List<OptionItem>();
        using var cn = _connectionFactory.CreateConnection();
        await cn.OpenAsync();

        using var cmd = new SqlCommand("SP_WS_DOCUMENT_TYPE_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
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



    public async Task<DocumentType> GetByIdAsync(long documentTypeId)
    {
        try
        {
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_DOCUMENT_TYPE_BY_ID", cn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };
            cmd.Parameters.AddWithValue("@DOCUMENT_TYPE_ID", documentTypeId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (reader.HasRows && await reader.ReadAsync())
            {
                return new DocumentType
                {
                    DocumentTypeId = (int)reader.GetInt64(0),
                    BusinessId = (int)reader.GetInt64(1),
                    CodeSunat = reader.GetString(2),
                    Description = reader.GetString(3)
                };
            }

            return null;
        }
        catch (SqlException ex)
        {
            throw new DatabaseException("Error al obtener tipo de documento por ID.", ex.Message);
        }
    }

    public async Task<bool> UpdateAsync(DocumentType doc)
    {
        try
        {
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_UPDATE_DOCUMENT_TYPE", cn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", doc.BusinessId);
            cmd.Parameters.AddWithValue("@DOCUMENT_TYPE_ID", doc.DocumentTypeId);
            cmd.Parameters.AddWithValue("@CODE_SUNAT", doc.CodeSunat);
            cmd.Parameters.AddWithValue("@DESCRIPTION", doc.Description);
            cmd.Parameters.AddWithValue("@UPDATE_USER", doc.UsersBy);

            using var reader = await cmd.ExecuteReaderAsync();
            return reader.HasRows && await reader.ReadAsync();
        }
        catch (SqlException ex)
        {
            throw new DatabaseException("Error al actualizar tipo de documento.", ex.Message);
        }
    }

    public async Task<bool> PatchStatusAsync(long documentTypeId, string status, int UsersBy, long businessId)
    {
        try
        {
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_UPDATE_DOCUMENT_TYPE_ACTIVE", cn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };

            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@DOCUMENT_TYPE_ID", documentTypeId);
            cmd.Parameters.AddWithValue("@UPDATE_USER", UsersBy);
            cmd.Parameters.AddWithValue("@STATUS", status);

            using var reader = await cmd.ExecuteReaderAsync();
            return reader.HasRows && await reader.ReadAsync();
        }
        catch (SqlException ex)
        {
            throw new DatabaseException("Error al cambiar el estado del tipo de documento.", ex.Message);
        }
    }

    public async Task<bool> ExistsAsync(string description,string codeSunat, int businessId, long? excludeId = null)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT COUNT(1) FROM DOCUMENT_TYPE WHERE BUSINESS_ID = @BID AND DESCRIPTION = @DESC";

            if (excludeId.HasValue)
            {
                sql += " AND DOCUMENT_TYPE_ID <> @ID";
            }

            using var cmd = new SqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@BID", businessId);
            cmd.Parameters.AddWithValue("@DESC", description);
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
            throw new DatabaseException("Error al validar existencia del tipo de documento.", ex.Message);
        }
    }

}
