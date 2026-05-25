using Application.DTOs.FileTracking;
using AutoMapper;
using Core.Entities;
using Core.Entities.paginations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Application.MappingProfiles
{
    public class FileTrackingProfile : Profile
    {
        public FileTrackingProfile()
        {
            CreateMap<FileTrackingOpporCreateDto, FileTracking>();
            CreateMap<FileTrackingProjectCreateDto, FileTracking>();
            CreateMap<FileTrackingProjectDeleteDto, FileTracking>();
            CreateMap<FileTrackingOpperDeleteDto, FileTracking>();

        }
    }
}
