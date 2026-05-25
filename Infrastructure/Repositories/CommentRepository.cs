using Core.Entities;
using Core.Interfaces;
using Core.Interfaces.Services;
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
    public class CommentRepository : ICommentRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        private readonly ILinkTokenService _linkTokenService;

        public CommentRepository(ISqlConnectionFactory connectionFactory, ILinkTokenService linkTokenService)
        {
            _connectionFactory = connectionFactory;
            _linkTokenService = linkTokenService;
        }

        public async Task<List<Comment>> ListAsync(long businessId, string linkToken, long userId, long areaId, long userInternalVisibilityId)
        {
            var result = new List<Comment>();

            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_LIST_OPPOR_COMMENTS", cn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", businessId);
            cmd.Parameters.AddWithValue("@OPPOR_ID", linkToken);
            cmd.Parameters.AddWithValue("@USERS_ID", userId);
            cmd.Parameters.AddWithValue("@AREA_ID", areaId);
            cmd.Parameters.AddWithValue("@USER_INTERNAL_VISIBILITY_ID", userInternalVisibilityId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {

                var commentId = reader.GetInt64(0);
                var opporId = reader.GetInt64(2);

                var audienceJson = reader.IsDBNull(reader.GetOrdinal("AUDIENCE"))
            ? "[]"
            : reader.GetString(reader.GetOrdinal("AUDIENCE"));

                var audienceIds = System.Text.Json.JsonSerializer.Deserialize<List<AreaAudience>>(audienceJson)
                                  ?.Select(x => x.AreaId).ToList()
                                  ?? new List<long>();

                result.Add(new Comment
                {
                    CommentToken = _linkTokenService.Issue("opportunity_comment", commentId, TimeSpan.FromHours(1)),
                    BusinessId = reader.GetInt64(reader.GetOrdinal("BUSINESS_ID")),
                    LinkToken = _linkTokenService.Issue("opportunity", opporId, TimeSpan.FromHours(1)),
                    Message = reader.GetString(reader.GetOrdinal("MESSAGE")),
                    CreatedBy = reader.GetInt64(reader.GetOrdinal("CREATED_BY")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CREATED_AT")),
                    VisibilityId = reader.GetInt64(reader.GetOrdinal("VISIBILITY_ID")),
                    AudienceAreaIds = audienceIds,
                    CreatedByName = reader.IsDBNull(reader.GetOrdinal("CREATED_BY_NAME"))
                ? null
                : reader.GetString(reader.GetOrdinal("CREATED_BY_NAME"))
                });
            }

            return result;
        }
        public async Task<Comment> CreateAsync(Comment model)
        {
            if (!_linkTokenService.TryValidate(model.LinkToken, out string entity, out long opporId))
            {
                throw new ArgumentException("El token proporcionado no es válido.");
            }

            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_CREATE_OPPOR_COMMENT", cn)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 15
            };
            cmd.Parameters.AddWithValue("@BUSINESS_ID", model.BusinessId);
            cmd.Parameters.AddWithValue("@OPPOR_ID", opporId);
            cmd.Parameters.AddWithValue("@MESSAGE", model.Message);
            cmd.Parameters.AddWithValue("@CREATED_BY", model.CreatedBy);
            cmd.Parameters.AddWithValue("@IS_INTERNAL", model.IsInternal);


            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var commentId = reader.GetInt64(0);
                var storedOpporId = reader.GetInt64(2);

                return new Comment
                {
                    CommentToken = _linkTokenService.Issue("comment", commentId, TimeSpan.FromHours(1)),
                    BusinessId = reader.GetInt64(reader.GetOrdinal("BUSINESS_ID")),
                    LinkToken = _linkTokenService.Issue("opportunity", storedOpporId, TimeSpan.FromHours(1)),
                    Message = reader.GetString(reader.GetOrdinal("MESSAGE")),
                    CreatedBy = reader.GetInt64(reader.GetOrdinal("CREATED_BY")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("CREATED_AT")),
                    CreatedByName = reader.IsDBNull(reader.GetOrdinal("CREATED_BY_NAME"))
                        ? null
                        : reader.GetString(reader.GetOrdinal("CREATED_BY_NAME")),
                };
            }

            return null;
        }

        public async Task MarkCommentsReadAsync(Comment model)
        {
            try
            {
                using var cn = _connectionFactory.CreateConnection();
                await cn.OpenAsync();
                using var cmd = new SqlCommand("SP_WS_MARK_OPPOR_COMMENTS_READ", cn)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 15
                };
                cmd.Parameters.AddWithValue("@BUSINESS_ID", model.BusinessId);
                cmd.Parameters.AddWithValue("@OPPOR_ID", model.LinkToken);
                cmd.Parameters.AddWithValue("@USERS_ID", model.CreatedBy);
                await cmd.ExecuteNonQueryAsync();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    "Error al marcar los comentarios como leídos en base de datos.",
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                throw new DatabaseException(
                    "Error inesperado al marcar los comentarios como leídos.",
                    ex.Message
                );
            }
        }
    }
}
