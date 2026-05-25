using Application.DTOs.Comment;
using Application.Services.RealTime;
using Application.UseCases.Notifications;
using AutoMapper;
using Core.Interfaces;
using Core.Interfaces.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Comment
{
    public class CreateComment
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;
        private readonly INotificationPush _push;


        public CreateComment(ICommentRepository repository, IMapper mapper, INotificationPush push)
        {
            _repository = repository;
            _mapper = mapper;
            _push = push;
        }
        public async Task<CommentDto> ExecuteAsync(CommentCreateDto model)
        {
            var entity = _mapper.Map<Core.Entities.Comment>(model);
            var created = await _repository.CreateAsync(entity);
            await RealtimeEvents.InvalidateBusinessAsync(
                _push,
                model.BusinessId,
                "opportunities"
            );
            return _mapper.Map<CommentDto>(created);
        }
    }
}
