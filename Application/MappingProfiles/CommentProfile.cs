using Application.DTOs.Comment;
using Application.UseCases.Comment;
using AutoMapper;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CommentCreateDto, Comment>()
                .ForMember(d => d.CommentToken, o => o.Ignore())        
                .ForMember(d => d.CreatedAt, o => o.Ignore())       
                .ForMember(d => d.CreatedByName, o => o.Ignore())   
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.UsersBy));

            CreateMap<Comment, CommentDto>();
            CreateMap<AreaAudience, AreaAudienceDto>();
            CreateMap<MarkCommentReadDto, Comment>();
        }
    }
}
