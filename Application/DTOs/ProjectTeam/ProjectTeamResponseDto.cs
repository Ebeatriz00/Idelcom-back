using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProjectTeam
{
    public class ProjectTeamResponseDto
    {
        public long ProjectTeamId { get; set; }
        public long ProjectId { get; set; }
        public string WorkerName { get; set; }
        public string JobTitle { get; set; }


    }
}
