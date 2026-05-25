using Application.DTOs.DocumentType;
using Application.DTOs.JobTitle;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles
{

    public class JobTitleProfile : Profile
    {
        public JobTitleProfile()
        {
            CreateMap<JobTitleCreateDto, JobTitle>();
            CreateMap<JobTitleUpdateDto, JobTitle>();
            CreateMap<DocumentType, DocumentTypeSelectDto>();
            CreateMap<JobTitle, JobTitleResponseDto>();
            CreateMap<JobTitle, JobTitleByIdDto>();

            CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
            CreateMap(typeof(PagedSelect<>), typeof(PagedSelect<>));
        }
    }
}
