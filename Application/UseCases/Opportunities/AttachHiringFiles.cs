using Application.DTOs.Opportunities;
using Core.Interfaces;
using SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Opportunities
{
    public class AttachHiringFiles
    {
        private readonly IOpportunitiesRepository _repository;

        public AttachHiringFiles(IOpportunitiesRepository repository)
        {
            _repository = repository;
        }

        public async Task<GlobalResponse> ExecuteAsync(long businessId, long opporId, long updateUser, List<OpportFiletrackingDto> files)
        {
            if (files == null || files.Count == 0)
                return new GlobalResponse { Status = 0, Message = "No se enviaron archivos." };

            var mapped = files.Select(f => new Core.Entities.OpportFiletracking
            {
                FileTitle = f.FileTitle,
                FileUrl = f.FileUrl,
                RelativePath = f.RelativePath,
                ArchiveType = f.ArchiveType
            }).ToList();

            return await _repository.AttachHiringFilesAsync(businessId, opporId, updateUser, mapped);


        }
    }

}
