using Core.Entities.paginations;
using Core.Interfaces;
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
    public class TypeAnalysisRepository : ITypeAnalysisRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        public TypeAnalysisRepository(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<PagedSelect<OptionItem>> GetTypeAnalysisForSelectAsync(string? search, int page, int pageSize)
        {
            var items = new List<OptionItem>();
            using var cn = _connectionFactory.CreateConnection();
            await cn.OpenAsync();

            using var cmd = new SqlCommand("SP_WS_TYPE_ANALYSIS_SELECT", cn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 15 };
            cmd.Parameters.AddWithValue("@SEARCH", (object?)(string.IsNullOrWhiteSpace(search) ? null : search.Trim()) ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PAGE", page);
            cmd.Parameters.AddWithValue("@PAGE_SIZE", pageSize);

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                items.Add(new OptionItem
                {
                    Value = dr.GetInt32(0),
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
    }
}
