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
    public class MarkCommentRead
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public MarkCommentRead(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task ExecuteAsync(MarkCommentReadDto model) { 
        
            var entity = _mapper.Map<Core.Entities.Comment>(model);
            await  _repository.MarkCommentsReadAsync(entity);
        }
    }
}
