using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICommentRepository
    {
        Task<List<Comment>> ListAsync(long businessId, string linkToken, long userId, long areaId, long userInternalVisibilityId);
        Task<Comment> CreateAsync(Comment entity);
        Task MarkCommentsReadAsync(Comment entity);

    }
}
