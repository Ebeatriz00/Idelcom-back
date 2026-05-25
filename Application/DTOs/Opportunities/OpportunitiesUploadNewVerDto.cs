using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Opportunities
{
    public class OpportunitiesUploadNewVerDto
    {
        public string LinkToken { get; set; } = default!;
        public long BusinessId { get; set; }
        public long UsersBy { get; set; }

        public string? ProposalComment { get; set; }

        public IFormFile? ExcelFile { get; set; }
        public string? FileTitle { get; set; }
        public string? FileUrl { get; set; }
        public string? RelativePath { get; set; }
        public string? ArchiveType { get; set; }

    }
}
