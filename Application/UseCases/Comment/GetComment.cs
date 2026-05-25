using Application.DTOs.Comment;
using AutoMapper;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Comment
{
    public class GetComment
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public GetComment(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<CommentDto>> ExecuteAsync(long businessId, string linkToken, long userId, long areaId, long userInternalVisibilityId)
        {
            var comments = await _repository.ListAsync(businessId, linkToken, userId, areaId, userInternalVisibilityId);
            if (comments == null || comments.Count == 0)
                return new List<CommentDto>();
            return _mapper.Map<List<CommentDto>>(comments);
        }
    }
}
